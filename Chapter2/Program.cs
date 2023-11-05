using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;

string endpoint = "https://YourUrl.cognitiveservices.azure.com/";
string key = "YourKey";

AzureKeyCredential credentials = new(key);
VisionServiceOptions visionOptions = new(endpoint, credentials);

string imageUrl = "https://bit.ly/engineer-costume";
using VisionSource source = VisionSource.FromUrl(imageUrl);

ImageAnalysisOptions analysisOptions = new() {
    Features = ImageAnalysisFeature.Caption
};

using ImageAnalyzer analyzer = new(visionOptions, source, analysisOptions);

ImageAnalysisResult result = analyzer.Analyze();

Console.WriteLine(result.Caption.Content);
