using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;

string endpoint = "https://YourUrl.cognitiveservices.azure.com/";
string key = "YourKey";

AzureKeyCredential credentials = new(key);
VisionServiceOptions visionOptions = new(endpoint, credentials);