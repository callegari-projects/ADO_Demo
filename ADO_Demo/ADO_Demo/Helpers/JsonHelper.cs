using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helpers
{
    public static class JsonHelper
    {
        // This per-class static object allows for detailed logging with singleton reuse. The datamember and property should be copied once per class to enable logging
       

        public static bool TryParse(Type expectedType, string jsonString, out object resultObject)
        {
            if (expectedType == null)
            {
                resultObject = null;
                return false;
            }
            if (string.IsNullOrEmpty(jsonString))
            {
                resultObject = null;
                return false;
            }


            try
            {
                resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, expectedType);
                return resultObject != null; // true only if the object is not null
            }
            catch (Exception ex)
            {
                string message = string.Format("JSON: {0}", jsonString);
                //log.Error(message, ex);

                resultObject = null;
                return false;
            }

            //deprecated older (less reliable) DataContractSerializer
            //          //use the JSON serializing object to convert this JSON text back into a known object
            //          DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(expectedType);
            //          //Get the specific type of the derived class
            //          using (MemoryStream myStream = new MemoryStream())
            //          {
            //              //Read the string into memory as a stream
            //              byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            //              myStream.Write(jsonBytes, 0, jsonBytes.Length);
            //              myStream.Position = 0;//set to look at the beginning
            //          
            //              try
            //              {
            //                  //Convert the object from the supplied JSON
            //                  resultObject = mySerializer.ReadObject(myStream);
            //                  return resultObject != null; // true only if the object is not null
            //              }
            //              catch (Exception ex)
            //              {
            //                  string message = string.Format("JSON: {0}", jsonString);
            //                  log.ErrorException(message, ex);
            //          
            //                  resultObject = null;
            //                  return false;
            //              }
            //          }

        }

        /// <summary>
        /// Returns the JSON representation of an object.
        /// If an object is null or an error occurs, this returns an empty string (eg... "")
        /// </summary>
        public static string GetJson(object theObject)
        {
            //validate
            if (theObject == null) { return ""; }

            string rawJsonResult = string.Empty;
            try
            {
                rawJsonResult = JsonConvert.SerializeObject(theObject);
            }
            catch (Exception ex)
            {
                string message = string.Format("error creating JSON: {0}", theObject);
                //log.Error(message, ex);
            }

            return rawJsonResult == null ? "" : rawJsonResult; // always return at least an empty string (non null)

            //deprecated older (less reliable) DataContractSerializer
            //              //DO the serialization
            //              DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(theObject.GetType());
            //              string rawJsonResult = "";
            //              using (MemoryStream myStream = new MemoryStream()) //using' handles disposal
            //              {
            //                  //create the JSON
            //                  try
            //                  {
            //                      mySerializer.WriteObject(myStream, theObject);
            //                  }
            //                  catch (Exception ex)
            //                  {
            //                      string message = string.Format("error Json serializing: {0}", theObject.GetType());
            //                      log.ErrorException(message, ex);
            //              
            //                      return "";
            //                  }
            //              
            //                  //Read the JSON in to a string
            //                  myStream.Position = 0;
            //                  StreamReader myReader = new StreamReader(myStream);
            //                  rawJsonResult = myReader.ReadToEnd();
            //              }
            //              
            //              return rawJsonResult == null ? "" : rawJsonResult; // always return at least an empty string (non null)
        }

    }
}
