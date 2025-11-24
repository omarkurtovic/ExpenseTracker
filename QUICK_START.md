# Quick Start Guide

## Current Status
- ✅ Live on Azure at [link]
- ✅ GitHub Actions CI/CD working
- ✅ Mobile-responsive design complete
- ✅ Multi-tenant user isolation verified
- ✅ Core features working (accounts, transactions, categories, tags)

## What's Next: 4-Week ML Roadmap

You're pivoting from polish/UI improvements to a **high-ROI learning goal**: building ML-powered smart categorization.

**Why?**
- Junior devs rarely have ML experience = huge differentiator
- Real user problem solved (saves 10+ min/week)
- 15-30% higher salary potential with ML skills
- Interview story: "I identified a problem and shipped ML to solve it"
- You have the foundation (Andrew Ng course + Python)

Read **`ML_ROADMAP.md`** for the full 4-week breakdown.

---

## Weekly Breakdown

### WEEK 1: Data & Foundation
**Effort**: 🟢 Medium | **Impact**: 🟢 High

```
Day 1-2:  Create ML.NET project, define data models
Day 3-4:  Extract transaction data, create data loading service
Day 5:    Validate data, prepare training dataset

Deliverable: CSV with 100+ transactions, ready to train
```

**Files to create:**
- `ExpenseTracker.ML/` (new C# class library project)
- `DataPreparationService.cs`
- `TransactionData.cs` (model for training)

**Git**: `feat: add ML.NET project and data preparation`

---

### WEEK 2: Model Building
**Effort**: 🟠 Medium-Hard | **Impact**: 🟠 Very High

```
Day 1-2:  Build ML pipeline, feature engineering
Day 3-4:  Train & evaluate model, measure accuracy
Day 5:    Handle edge cases (empty descriptions, new merchants)

Goal: Achieve 80%+ accuracy
Deliverable: Trained model.zip file with metrics
```

**Key code pattern:**
```csharp
var pipeline = mlContext.Transforms
    .Text.FeaturizeText("DescriptionFeaturized", "Description")
    .Append(mlContext.Transforms.Concatenate("Features", ...))
    .Append(mlContext.MulticlassClassification.Trainers
        .SdcaMaximumEntropy(...));

var model = pipeline.Fit(trainingData);
```

**Git**: 
- `feat: build and train category prediction model`
- `docs: model evaluation metrics and accuracy`

---

### WEEK 3: Integration
**Effort**: 🟢 Medium | **Impact**: 🟠 Very High

```
Day 1-2:  Create CategorizationService, register in DI
Day 3-4:  Update TransactionDialog to show suggestions
Day 5:    Build feedback tracking system

Deliverable: Suggestions showing in UI with confidence scores
```

**Key UI change:**
```razor
@if (suggestedCategoryId.HasValue)
{
    <MudSelectItem Value="suggestedCategoryId.Value">
        ✨ @categoryName (confidence: @confidence.ToString("P"))
    </MudSelectItem>
}
```

**Git**:
- `feat: add categorization service and suggestions`
- `feat: implement feedback tracking`

---

### WEEK 4: Deployment & Documentation
**Effort**: 🟢 Easy-Medium | **Impact**: 🟠 High

```
Day 1-2:  Performance optimization, caching
Day 3:    Add anomaly detection (optional bonus)
Day 4:    Write tests, ensure model inference < 100ms
Day 5:    Deploy to Azure, publish blog post

Deliverable: Live on Azure + published blog
```

**Blog outline:**
1. Problem statement (manual categorization tedious)
2. Solution design (train on historical data)
3. Implementation walkthrough (code samples)
4. Results (85% accuracy, user feedback)
5. Lessons learned (class imbalance, performance tuning)

**Git**: `feat: optimize and deploy ML features` + `blog: ML roadmap blog post`

---

## Success Criteria

### By End of Week 2
- [ ] Model trained and evaluated
- [ ] 80%+ accuracy achieved
- [ ] Metrics documented
- [ ] Edge cases handled

### By End of Week 3
- [ ] Suggestions showing in UI
- [ ] Feedback being logged
- [ ] No regressions in existing features
- [ ] Performance acceptable

### By End of Week 4
- [ ] Live on Azure
- [ ] Tests passing
- [ ] Blog post published
- [ ] Interview story solid

---

## Common Pitfalls

| Issue | Fix |
|-------|-----|
| Model too slow | Cache prediction engine, batch operations, profile inference time |
| Low accuracy | Add more features (time, amount patterns), use ensemble methods |
| Data imbalanced | Use stratified sampling, weighted loss functions |
| Inference in UI feels sluggish | Async/await, debounce description changes |
| Forgot to handle nulls | Check for empty descriptions, use fallback logic |

---

## Commands You'll Need

```powershell
# Create ML project
dotnet new classlib -n ExpenseTracker.ML
cd ExpenseTracker.ML
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.Transforms

# Add project to solution
cd ..
dotnet sln add ExpenseTracker.ML

# Test specific class
dotnet test --filter "CategoryPredictionTests"

# Publish to Azure
git add .
git commit -m "feat: add ML categorization"
git push azure main
```

---

## Mental Model: How ML.NET Works

```
1. Prepare Data
   ↓
   Transaction CSV → Normalize, featurize text, add numeric features
   
2. Build Pipeline
   ↓
   Data transformers (FeaturizeText, Concatenate) 
   + Trainer (SdcaMaximumEntropy for multiclass)
   
3. Train Model
   ↓
   Feed training data through pipeline
   Model learns patterns between features and categories
   
4. Evaluate
   ↓
   Test on held-out data
   Check accuracy, confusion matrix, per-class metrics
   
5. Deploy
   ↓
   Save model.zip
   Load in production, use PredictionEngine for inference
   
6. Feedback Loop
   ↓
   User corrects prediction
   Log as feedback
   Retrain monthly with updated data
```

---

## Interview Talking Points

**Problem**: Users spending 10+ minutes/week manually categorizing transactions

**Solution**: Train ML model on historical data to suggest categories

**Results**: 
- 85% accuracy
- < 50ms inference time
- Users love it, saves time

**Technical Challenges Solved**:
1. **Imbalanced data** → Used stratified sampling
2. **Performance** → Cached prediction engine
3. **Confidence handling** → Only suggest if > 0.7 confidence
4. **Continuous improvement** → Feedback loop for retraining

**Follow-up Answers Ready**:
- "How would you handle new categories?" → Retrain, use zero-shot classification
- "What if accuracy drops?" → Monitor feedback ratio, retrain weekly
- "Scale to 10k users?" → Batch prediction, async UI, model compression

---

## Resources

- **ML.NET Docs**: https://learn.microsoft.com/dotnet/machine-learning/
- **Multi-class Classification**: https://learn.microsoft.com/dotnet/machine-learning/tutorials/sentiment-analysis
- **Full Roadmap**: See `ML_ROADMAP.md` for day-by-day breakdown

---

## When You Get Stuck

**Problem**: Model accuracy is 65%
**Next step**: 
1. Check data quality (nulls, duplicates)
2. Add more features (time of day, amount, day of week)
3. Try different trainer (LightGbm vs SdcaMaximumEntropy)

**Problem**: Inference takes 500ms
**Next step**:
1. Profile: Is it loading model or predicting?
2. Cache the prediction engine
3. Batch predictions instead of one-at-a-time

**Problem**: UI doesn't show suggestions
**Next step**:
1. Check CategorizationService is registered in DI
2. Verify PredictCategory returns correct type
3. Add debug logging: `Console.WriteLine($"Prediction: {prediction.PredictedCategoryId}")`

---

## You're Ready

You've shipped a responsive, full-stack Blazor app. Now you're adding ML to make it smarter.

This 4-week sprint will give you:
- ✅ Production ML experience
- ✅ C# + ML.NET skills
- ✅ Interview story
- ✅ Blog post for portfolio
- ✅ Salary bump when hired 💰

**Start Week 1 Day 1 today.**
