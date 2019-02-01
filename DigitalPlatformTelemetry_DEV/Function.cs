using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text;
using System.IO;
using Amazon.KinesisFirehose;
using Amazon.KinesisFirehose.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DigitalPlatformTelemetry_DEV
{

    public class TelemetryList
    {
        public IList<Telemetry> Telemetry { get; set; }
    }

    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            APIGatewayProxyResponse APIResponse = new APIGatewayProxyResponse();
            try
            {
                    /*Json to Telemetry List*/
                    TelemetryList telemetryList = JsonConvert.DeserializeObject<TelemetryList>(request.Body);
                    int ItemCount = telemetryList.Telemetry.Count;

                    /*Get Environment Vars*/
                    string EVDeliveryStream = Environment.GetEnvironmentVariable("FIREHOSE_DELIVERYSTREAM");
                    string EVRegion = Environment.GetEnvironmentVariable("REGION");
                    bool EVSendToFirehose = Environment.GetEnvironmentVariable("PUSH_TO_FIREHOSE").ToUpper() == "TRUE";

                    if (ItemCount > 0 && EVSendToFirehose) // Only Processes if ItemCount is >0 and SentToFirehose==True
                    {
                        byte[] oByte;

                        int CountProcessed = 0;


                        //Region Validation
                        Amazon.RegionEndpoint Region;
                        if (EVRegion==null || EVRegion.Length == 0)
                        {
                            Region = Amazon.RegionEndpoint.GetBySystemName("us-east-1");
                        }
                        else
                        {
                            Region = Amazon.RegionEndpoint.GetBySystemName(EVRegion);
                        }

                        for (int i = 0; i < ItemCount; i++) {
                            telemetryList.Telemetry[i].Instant = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                            oByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(telemetryList.Telemetry[i]));
                            using (MemoryStream ms = new MemoryStream(oByte))
                            {
                                Record RecordToSend = new Record
                                {
                                    Data = ms
                                };

                                PutRecordRequest DeliveryRequest = new PutRecordRequest
                                {
                                    DeliveryStreamName = EVDeliveryStream, //DigitalPlatformTelemetry
                                    Record = RecordToSend
                                };

                                AmazonKinesisFirehoseClient FirehoseClient = new AmazonKinesisFirehoseClient(Region);

                                PutRecordResponse Response = FirehoseClient.PutRecord(DeliveryRequest);

                                CountProcessed +=1;

                            }

                        }

                        APIResponse.StatusCode = 200;
                        APIResponse.Body = "Sucess, processed " + CountProcessed + " items " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        return APIResponse;

                    }
                    APIResponse.StatusCode = 200;
                    APIResponse.Body = APIResponse.Body = "Sucess, no entries processed. " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    return APIResponse;
              
            }
            catch (InvalidCastException e)
            {
                
                APIResponse.StatusCode = 500;
                APIResponse.Body = e.Message;
                return APIResponse;
            }
        }
    }
}