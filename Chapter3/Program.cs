using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;

string endpoint = "https://YourUrl.cognitiveservices.azure.com/";
string key = "YourKey";

AzureKeyCredential credentials = new(key);
VisionServiceOptions visionOptions = new(endpoint, credentials);

string filePath = Path.Combine(Environment.CurrentDirectory, "Penmanship.jpg");
using VisionSource source = VisionSource.FromFile(filePath);

ImageAnalysisOptions analysisOptions = new() {
    Features = ImageAnalysisFeature.Text,
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
    // Read lines
    DetectedText text = result.Text;
    foreach (DetectedTextLine line in text.Lines) 
    {
        Console.WriteLine($"{line.Content}");
    }

    Console.WriteLine();

    // Read words with confidences
    foreach (DetectedTextLine line in text.Lines) 
    {
        foreach (DetectedTextWord word in line.Words) 
        {
            Console.Write($"{word.Content} ({word.Confidence:P2}) ");
        }
        Console.WriteLine();
    }
}