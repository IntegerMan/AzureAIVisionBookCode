using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using System;

string endpoint = "https://YourUrl.cognitiveservices.azure.com/";
string key = "YourKey";

AzureKeyCredential credentials = new(key);
VisionServiceOptions visionOptions = new(endpoint, credentials);

string filePath = Path.Combine(Environment.CurrentDirectory, "MadScience.jpg");
using VisionSource source = VisionSource.FromFile(filePath);

//string imageUrl = "https://bit.ly/engineer-costume";
//using VisionSource source = VisionSource.FromUrl(imageUrl);

Console.WriteLine("Azure AI Vision Image Analysis Demos");

Console.WriteLine();
Console.WriteLine("1) Analyze Image");
Console.WriteLine("2) Background Removal");
Console.WriteLine("3) Foreground Matting");
Console.WriteLine();
Console.Write("What would you like to do? ");

string choice = Console.ReadLine()!.ToUpper().Trim();

Console.WriteLine();

switch (choice) {
    case "1":
        AnalyzeImage();
        break;

    case "2":
        RemoveBackground();
        break;

    case "3":
        ForegroundMatting();
        break;

    default:
        Console.WriteLine("Invalid choice");
        break;
}

Console.WriteLine();
Console.WriteLine("Thank you for using the Azure AI Vision Demo");

void AnalyzeImage() {
    ImageAnalysisOptions analysisOptions = new() {
        Features = ImageAnalysisFeature.Caption |
                   ImageAnalysisFeature.DenseCaptions |
                   ImageAnalysisFeature.Tags |
                   ImageAnalysisFeature.Objects |
                   ImageAnalysisFeature.People |
                   ImageAnalysisFeature.CropSuggestions,
        GenderNeutralCaption = true,
        Language = "en",
        CroppingAspectRatios = new[] { 0.75, 1.0, 1.25, 1.5, 1.8 }
    };

    using ImageAnalyzer analyzer = new(visionOptions, source, analysisOptions);

    ImageAnalysisResult result = analyzer.Analyze();

    if (result.Reason == ImageAnalysisResultReason.Error) {
        ImageAnalysisErrorDetails error = ImageAnalysisErrorDetails.FromResult(result);
        Console.WriteLine($"{error.Message} ({error.ErrorCode})");
    } else if (result.Reason == ImageAnalysisResultReason.Analyzed) {
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

        Console.WriteLine("Suggesting Crops:");
        foreach (CropSuggestion suggestion in result.CropSuggestions) {
            Console.WriteLine($"{suggestion.AspectRatio} @ {suggestion.BoundingBox}");
        }
    }
}

void RemoveBackground() {
    ImageAnalysisOptions analysisOptions = new() {
        Features = ImageAnalysisFeature.None,
        SegmentationMode = ImageSegmentationMode.BackgroundRemoval
    };

    using ImageAnalyzer analyzer = new(visionOptions, source, analysisOptions);

    ImageAnalysisResult result = analyzer.Analyze();

    if (result.Reason == ImageAnalysisResultReason.Error) {
        ImageAnalysisErrorDetails error = ImageAnalysisErrorDetails.FromResult(result);
        Console.WriteLine($"{error.Message} ({error.ErrorCode})");
    } else if (result.Reason == ImageAnalysisResultReason.Analyzed) {
        int width = result.SegmentationResult.ImageWidth;
        int height = result.SegmentationResult.ImageWidth;

        Console.WriteLine($"Analyzed image with a {width}x{height}px result");

        File.WriteAllBytes("BackgroundRemoval.png", result.SegmentationResult.ImageBuffer.ToArray());
    }
}

void ForegroundMatting() {
    ImageAnalysisOptions analysisOptions = new() {
        Features = ImageAnalysisFeature.None,
        SegmentationMode = ImageSegmentationMode.ForegroundMatting
    };

    using ImageAnalyzer analyzer = new(visionOptions, source, analysisOptions);

    ImageAnalysisResult result = analyzer.Analyze();

    if (result.Reason == ImageAnalysisResultReason.Error) {
        ImageAnalysisErrorDetails error = ImageAnalysisErrorDetails.FromResult(result);
        Console.WriteLine($"{error.Message} ({error.ErrorCode})");
    } else if (result.Reason == ImageAnalysisResultReason.Analyzed) {
        int width = result.SegmentationResult.ImageWidth;
        int height = result.SegmentationResult.ImageWidth;

        Console.WriteLine($"Analyzed image with a {width}x{height}px result");

        File.WriteAllBytes("ForegroundMatting.png", result.SegmentationResult.ImageBuffer.ToArray());
    }
}