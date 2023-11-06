using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using System;

string endpoint = "https://YourUrl.cognitiveservices.azure.com/";
string key = "YourKey";

AzureKeyCredential credentials = new(key);
VisionServiceOptions visionOptions = new(endpoint, credentials);


string filePath = Path.Combine(Environment.CurrentDirectory, "DogOS.jpg");
using VisionSource source = VisionSource.FromFile(filePath);

//string imageUrl = "https://bit.ly/engineer-costume";
//using VisionSource source = VisionSource.FromUrl(imageUrl);

ImageAnalysisOptions analysisOptions = new() {
    Features = ImageAnalysisFeature.Caption |
               ImageAnalysisFeature.DenseCaptions |
               ImageAnalysisFeature.Tags |
               ImageAnalysisFeature.Objects | 
               ImageAnalysisFeature.People,    
    GenderNeutralCaption = true,
    Language = "en",
};

using ImageAnalyzer analyzer = new(visionOptions, source, analysisOptions);

ImageAnalysisResult result = analyzer.Analyze();

if (result.Reason == ImageAnalysisResultReason.Error) 
{
    ImageAnalysisErrorDetails error = ImageAnalysisErrorDetails.FromResult(result);
    Console.WriteLine($"{error.Message} ({error.ErrorCode})");
} 
else if (result.Reason == ImageAnalysisResultReason.Analyzed) 
{
    Console.WriteLine($"Caption: {result.Caption.Content}");

    Console.WriteLine("Detecting Dense Captions:");
    foreach (ContentCaption cap in result.DenseCaptions) {
        string content = cap.Content;
        string box = cap.BoundingBox.ToString();
        Console.WriteLine($"{content} at {box} (Conf {cap.Confidence:p2})");
    }

    Console.WriteLine("Detecting Tags:");
    foreach (ContentTag tag in result.Tags) {
        Console.WriteLine($"{tag.Name} (Confidence: {tag.Confidence:p2})");
    }

    Console.WriteLine("Detecting Objects:");
    foreach (DetectedObject obj in result.Objects) {
        Console.WriteLine($"{obj.Name} {obj.BoundingBox}  (Conf: {obj.Confidence:p2})");
    }

    Console.WriteLine("Detecting People:");
    foreach (DetectedPerson person in result.People) {
        Console.WriteLine($"{person.BoundingBox}  (Conf: {person.Confidence:p2})");
    }

}
