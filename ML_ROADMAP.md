# 🤖 ML-Powered Expense Tracker: 4-Week Roadmap

## The Vision
Transform the expense tracker with **smart transaction categorization** using machine learning. Users get category suggestions based on transaction descriptions, and the model improves as they provide feedback.

---

## Why This Matters

### For Your Career
- ✅ ML experience (rare for junior devs = huge differentiator)
- ✅ Full-stack + ML on resume (15-30% salary bump)
- ✅ Interview story: "I identified a user pain point and solved it with ML"
- ✅ Production ML (not just Jupyter notebooks)

### For Users
- ✅ Saves time (no manual categorization)
- ✅ Gets smarter over time (feedback loop)
- ✅ Actually useful (not a gimmick)

### For Your Portfolio
- ✅ Shows depth (not just another CRUD feature)
- ✅ GitHub shows interesting commits
- ✅ Blog post opportunity (machine learning in C#)

---

## 4-Week Execution Plan

### WEEK 1: Foundation & Data Preparation
**Goal**: Have clean training data ready

#### Day 1-2: Setup ML.NET Project
```csharp
// Create new project
dotnet new classlib -n ExpenseTracker.ML
cd ExpenseTracker.ML

// Add NuGet packages
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.Transforms
```

Create data models:
```csharp
// TransactionData.cs - for training
public class TransactionData
{
    public string Description { get; set; }
    public float Amount { get; set; }
    public int Hour { get; set; }  // Transaction time
    public int DayOfWeek { get; set; }
    
    [ColumnName("Label")]
    public uint CategoryId { get; set; }
}

// ModelOutput.cs - for predictions
public class ModelOutput
{
    [ColumnName("PredictedLabel")]
    public uint PredictedCategoryId { get; set; }
    
    public float[] Score { get; set; }  // Confidence for each category
}
```

#### Day 3-4: Extract & Prepare Data
Create data loading service:
```csharp
public class DataPreparationService
{
    private readonly AppDbContext _context;
    
    public List<TransactionData> GetTrainingData(string userId)
    {
        return _context.Transactions
            .Where(t => t.Account.IdentityUserId == userId)
            .Select(t => new TransactionData
            {
                Description = t.Description ?? "",
                Amount = (float)t.Amount,
                Hour = t.Date.Hour,
                DayOfWeek = (int)t.Date.DayOfWeek,
                CategoryId = (uint)t.CategoryId
            })
            .ToList();
    }
}
```

**Checklist:**
- [ ] ML.NET project created
- [ ] Data models defined
- [ ] Data loading service working
- [ ] Training data exports to CSV (for inspection)

#### Day 5: Data Pipeline
- [ ] Validate data (no nulls, proper distribution)
- [ ] Check category distribution (are some heavily skewed?)
- [ ] Document data statistics
- [ ] Create seed data for testing

**Deliverable**: CSV file with 100+ transactions, ready for training

---

### WEEK 2: Model Building & Training
**Goal**: Trained model with 80%+ accuracy

#### Day 1-2: Build ML Pipeline
```csharp
public class CategoryPredictionModel
{
    private MLContext _mlContext;
    
    public ITransformer TrainModel(List<TransactionData> trainingData)
    {
        _mlContext = new MLContext(seed: 1);
        
        // Load data
        var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
        
        // Create pipeline
        var pipeline = _mlContext.Transforms
            // Featurize text (merchant name/description)
            .Text.FeaturizeText("DescriptionFeaturized", "Description")
            
            // Add numeric features
            .Append(_mlContext.Transforms.Concatenate(
                "Features",
                "DescriptionFeaturized", "Amount", "Hour", "DayOfWeek"))
            
            // Use multiclass classifier
            .Append(_mlContext.MulticlassClassification.Trainers
                .SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"));
        
        // Train
        var model = pipeline.Fit(dataView);
        return model;
    }
}
```

#### Day 3-4: Evaluate Model
```csharp
public class ModelEvaluator
{
    public void EvaluateModel(ITransformer model, List<TransactionData> testData)
    {
        var predictions = model.Transform(_mlContext.Data.LoadFromEnumerable(testData));
        
        var metrics = _mlContext.MulticlassClassification.Evaluate(
            predictions,
            labelColumnName: "Label",
            predictedLabelColumnName: "PredictedLabel");
        
        Console.WriteLine($"Accuracy: {metrics.MacroAccuracy:P}");
        Console.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy:P}");
        
        // If accuracy < 80%, adjust hyperparameters
        if (metrics.MacroAccuracy < 0.80)
        {
            // Try different trainer or features
        }
    }
}
```

**Metrics to track:**
- Macro Accuracy (average across all categories)
- Micro Accuracy (overall)
- Per-category precision/recall
- Confusion matrix (which categories get confused?)

#### Day 5: Handle Edge Cases
- [ ] What if category has <5 samples? (exclude from training)
- [ ] What if description is empty? (use amount + time)
- [ ] What about new/unknown merchants? (model gracefully degrades)

**Deliverable**: Trained model file (`.zip`), documented accuracy metrics

---

### WEEK 3: Integration & Feedback Loop
**Goal**: Model suggestions showing in UI

#### Day 1-2: Create Categorization Service
```csharp
public class CategorizationService
{
    private ITransformer _model;
    private PredictionEngine<TransactionData, ModelOutput> _predictionEngine;
    
    public CategorizationService(string modelPath)
    {
        var mlContext = new MLContext();
        _model = mlContext.Model.Load(modelPath, out var modelInputSchema);
        _predictionEngine = mlContext.Model.CreatePredictionEngine<TransactionData, ModelOutput>(_model);
    }
    
    public CategoryPrediction PredictCategory(string description, float amount, int hour)
    {
        var input = new TransactionData
        {
            Description = description,
            Amount = amount,
            Hour = hour,
            DayOfWeek = (int)DateTime.Now.DayOfWeek
        };
        
        var output = _predictionEngine.Predict(input);
        var confidence = output.Score.Max();
        
        return new CategoryPrediction
        {
            PredictedCategoryId = (int)output.PredictedCategoryId,
            Confidence = confidence,
            AllScores = output.Score
        };
    }
}

public class CategoryPrediction
{
    public int PredictedCategoryId { get; set; }
    public float Confidence { get; set; }
    public float[] AllScores { get; set; }
}
```

Register in DI:
```csharp
// Program.cs
builder.Services.AddScoped(sp => 
    new CategorizationService(Path.Combine(AppContext.BaseDirectory, "Models", "model.zip")));
```

#### Day 3-4: UI Integration
Update `TransactionDialog.razor`:
```razor
@inject CategorizationService CategorizationService

<MudSelect @bind-Value="TransactionDto.CategoryId" Label="Category">
    @if (suggestedCategoryId.HasValue && suggestedCategoryId != TransactionDto.CategoryId)
    {
        <MudSelectItem Value="suggestedCategoryId.Value">
            <MudText Color="Color.Success">
                ✨ @GetCategoryName(suggestedCategoryId.Value) 
                (confidence: @suggestedConfidence.ToString("P"))
            </MudText>
        </MudSelectItem>
    }
    @foreach (var category in categories)
    {
        <MudSelectItem Value="@category.Id">@category.Name</MudSelectItem>
    }
</MudSelect>

@code {
    private int? suggestedCategoryId;
    private float suggestedConfidence;
    
    private async Task OnDescriptionChanged(string description)
    {
        if (string.IsNullOrEmpty(description)) return;
        
        var prediction = CategorizationService.PredictCategory(
            description,
            TransactionDto.Amount ?? 0,
            DateTime.Now.Hour
        );
        
        // Only suggest if confident
        if (prediction.Confidence > 0.7)
        {
            suggestedCategoryId = prediction.PredictedCategoryId;
            suggestedConfidence = prediction.Confidence;
        }
    }
}
```

#### Day 5: Feedback Tracking
Create feedback model:
```csharp
public class CategoryFeedback
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    public int PredictedCategoryId { get; set; }
    public int ActualCategoryId { get; set; }
    public float Confidence { get; set; }
    public DateTime Timestamp { get; set; }
}
```

Add to DbContext and migration:
```csharp
// AppDbContext
public DbSet<CategoryFeedback> CategoryFeedbacks { get; set; }

// Migration
dotnet ef migrations add AddCategoryFeedback --project ExpenseTrackerWebApp
dotnet ef database update
```

Service to log feedback:
```csharp
public class FeedbackService
{
    public async Task RecordPredictionFeedbackAsync(
        int transactionId, 
        int predictedCategoryId, 
        int actualCategoryId,
        float confidence)
    {
        var feedback = new CategoryFeedback
        {
            TransactionId = transactionId,
            PredictedCategoryId = predictedCategoryId,
            ActualCategoryId = actualCategoryId,
            Confidence = confidence,
            Timestamp = DateTime.Now
        };
        
        _context.CategoryFeedbacks.Add(feedback);
        await _context.SaveChangesAsync();
    }
}
```

**Deliverable**: Suggestions showing in UI, feedback logged

---

### WEEK 4: Polish & Deploy
**Goal**: Production-ready, deployed to Azure

#### Day 1-2: Performance Optimization
```csharp
// Cache model loading
public class CachedCategorizationService
{
    private static readonly Lazy<PredictionEngine<TransactionData, ModelOutput>> 
        LazyPredictionEngine = new Lazy<PredictionEngine<TransactionData, ModelOutput>>(() =>
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load("model.zip", out _);
            return mlContext.Model.CreatePredictionEngine<TransactionData, ModelOutput>(model);
        });
    
    public CategoryPrediction PredictCategory(string description, float amount)
    {
        // Reuse engine - no reload each time
        var output = LazyPredictionEngine.Value.Predict(new TransactionData 
        { 
            Description = description,
            Amount = amount 
        });
        
        return new CategoryPrediction 
        { 
            PredictedCategoryId = (int)output.PredictedCategoryId,
            Confidence = output.Score.Max()
        };
    }
}
```

Measure inference time:
```csharp
// Benchmark
var sw = Stopwatch.StartNew();
for (int i = 0; i < 1000; i++)
{
    var prediction = service.PredictCategory("Whole Foods", 50.99f);
}
sw.Stop();
Console.WriteLine($"1000 predictions: {sw.ElapsedMilliseconds}ms avg {sw.ElapsedMilliseconds/1000f}ms per");
// Target: < 100ms per prediction
```

#### Day 3: Add Anomaly Detection (Bonus)
```csharp
public class AnomalyService
{
    public bool IsUnusualSpending(float amount, float userAverageSpending)
    {
        // Flag if >2 std deviations from average
        return amount > userAverageSpending * 2.5f;
    }
}
```

#### Day 4: Testing
Add unit tests:
```csharp
[Fact]
public void HighConfidencePrediction_Returns_Suggestion()
{
    var service = new CategorizationService("model.zip");
    var prediction = service.PredictCategory("Amazon purchase", 50f);
    
    Assert.True(prediction.Confidence > 0.7);
    Assert.NotNull(prediction.PredictedCategoryId);
}

[Fact]
public void LowConfidencePrediction_Returns_NoSuggestion()
{
    var service = new CategorizationService("model.zip");
    var prediction = service.PredictCategory("xyzabc 123", 0.01f);
    
    Assert.True(prediction.Confidence < 0.5);
}
```

#### Day 5: Deploy & Blog

Deploy to Azure:
```bash
# Build & publish
dotnet publish -c Release

# Deploy ML model with app
# Ensure model.zip is included in publish output

# Push to Azure
git add .
git commit -m "feat: add ML-powered category predictions"
git push azure main
```

Write blog post: **"Building ML-Powered Expense Categorization with ML.NET"**

Structure:
1. Problem: Manual categorization is tedious
2. Solution: Train model on historical transactions
3. Implementation: Data prep → model training → integration
4. Results: Achieved 85% accuracy, users love suggestions
5. Lessons learned: Handling imbalanced data, inference performance, feedback loops

**Deliverable**: Live on Azure with blog post

---

## Tech Stack Reference

```
Language: C# 12
Framework: .NET 8
UI: Blazor Server + MudBlazor

ML:
  - Microsoft.ML 3.0+
  - Microsoft.ML.Transforms
  - Microsoft.ML.Vision (optional, for future enhancements)

Database:
  - SQLite (existing)
  - Add CategoryFeedback table
  - Add CategorySuggestion table (optional logging)
```

---

## Success Metrics

### Technical
- [ ] Model accuracy > 80%
- [ ] Inference time < 100ms
- [ ] Handles 50+ categories
- [ ] Confidence scoring accurate
- [ ] Deployed without errors

### User Experience
- [ ] Suggestions appear instantly
- [ ] Users see confidence score
- [ ] Feedback mechanism working
- [ ] No performance impact on app

### Portfolio
- [ ] Blog post published (500+ words)
- [ ] GitHub history shows clean commits
- [ ] Can explain model decisions in interview
- [ ] LinkedIn post showcasing feature

---

## Interview Story (Ready by Week 5)

```
"I analyzed our user data and found that manual transaction categorization 
was taking users 10-15 minutes per week. I trained an ML.NET model on 
historical transaction data to predict categories based on merchant 
descriptions and transaction patterns.

The model achieved 85% accuracy on test data. I integrated it into the 
app to suggest categories with confidence scores. When users correct 
predictions, I log that feedback for future retraining.

The biggest challenge was handling imbalanced data—some categories had 
way more samples than others. I used stratified sampling and class 
weighting to address this.

The model inference runs in <50ms, so it doesn't impact app performance. 
It's now deployed to production and users consistently give positive 
feedback on time savings."

[Now they ask follow-ups like: "How would you handle new categories?" 
"What if user data changes significantly?" - These are great problems to solve]
```

---

## Common Challenges & Solutions

| Challenge | Solution |
|-----------|----------|
| Low accuracy | Add more features (time patterns, amount), use ensemble methods |
| Model too slow | Batch predictions, cache in memory, consider model compression |
| Imbalanced data | Stratified sampling, weighted loss, synthetic oversampling |
| Unclear predictions | Add confidence thresholds, only suggest if > 0.7 |
| Model drift | Track prediction vs actual over time, retrain weekly |
| New merchants | Fallback to user's favorite category or amount-based heuristic |

---

## Git Commit Timeline

```
Week 1:
  - feat: add ML.NET project and data preparation
  - feat: create transaction data loading service

Week 2:
  - feat: build and train category prediction model
  - docs: model accuracy and evaluation metrics

Week 3:
  - feat: integrate categorization service
  - feat: show category suggestions in transaction dialog
  - feat: add category feedback tracking

Week 4:
  - feat: optimize model inference performance
  - feat: add anomaly detection (bonus)
  - feat: deploy ML features to Azure
  - blog: published ML roadmap blog post
```

---

## Resources

- [ML.NET Documentation](https://learn.microsoft.com/en-us/dotnet/machine-learning/)
- [Andrew Ng's ML Specialization Concepts](https://www.coursera.org/specializations/machine-learning-introduction) (apply these!)
- [ML.NET Samples](https://github.com/dotnet/machinelearning-samples)
- [Multi-class Classification Tutorial](https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis)

---

## You've Got This

This is a realistic, achievable goal that will genuinely differentiate you as a junior dev. 

**Week 1**: Data prep (boring but necessary)
**Week 2**: Training (watching accuracy climb is satisfying)
**Week 3**: Integration (seeing it work in the app is cool)
**Week 4**: Polish & blog (celebrating your work)

4 weeks. Let's go.
