# Blazor Form Validation Guide

## Overview
This document explains the form validation implementation in the Directus Blazor application, following Blazor best practices.

## Best Practices Implemented

### 1. Separate Model Classes
All form models are defined in separate files in the `Models` folder:
- `LoginModel.cs` - Login form data
- `ItemsQueryModel.cs` - Items browser query form data

**Why**: Separating models from components improves:
- Code organization and maintainability
- Reusability across multiple components
- Testability

### 2. Data Annotations for Validation
Each model uses Data Annotations to define validation rules:

```csharp
public class LoginModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(1, ErrorMessage = "Password cannot be empty")]
    public string Password { get; set; } = string.Empty;
}
```

**Benefits**:
- Declarative validation rules
- Client-side and server-side validation
- Custom error messages
- Reusable validation logic

### 3. EditForm with DataAnnotationsValidator
All forms use the `<EditForm>` component with `<DataAnnotationsValidator>`:

```razor
<EditForm Model="@loginModel" OnValidSubmit="HandleLoginAsync">
    <DataAnnotationsValidator />
    <!-- form fields -->
</EditForm>
```

**Why**: 
- Enables automatic validation based on Data Annotations
- Triggers validation on form submission
- Provides validation state to child components

### 4. ValidationMessage Components
Each input field has an associated `<ValidationMessage>`:

```razor
<InputText id="email" 
           @bind-Value="loginModel.Email" 
           class="form-control" />
<ValidationMessage For="@(() => loginModel.Email)" class="text-danger" />
```

**Benefits**:
- Shows field-specific error messages
- Automatically displays/hides based on validation state
- Strongly-typed with lambda expressions

### 5. Proper Model Initialization
Models are initialized in `OnInitialized()`:

```csharp
protected override void OnInitialized()
{
    loginModel = new LoginModel();
}
```

**Why**:
- Ensures clean state on component initialization
- Prevents null reference exceptions
- Allows for default values

### 6. StateHasChanged() Calls
UI updates are triggered with `StateHasChanged()`:

```csharp
try
{
    // perform operation
    StateHasChanged();
}
catch (Exception ex)
{
    errorMessage = ex.Message;
    StateHasChanged();
}
```

**Why**:
- Forces UI refresh after async operations
- Ensures error messages are displayed immediately
- Improves user experience with real-time feedback

### 7. Accessibility Features
Forms include proper accessibility attributes:

```razor
<InputText id="email" 
           autocomplete="email" />
<InputText id="password" 
           type="password"
           autocomplete="current-password" />
```

**Benefits**:
- Better browser autofill support
- Improved security (password managers)
- Compliance with accessibility standards

## Forms in the Application

### Login Form (`/login`)
**Location**: `Components/Pages/Login.razor`
**Model**: `Models/LoginModel.cs`

**Fields**:
- Email (required, valid email format)
- Password (required, min length 1)

**Features**:
- Client-side validation
- Loading state during submission
- Success/error messages
- Automatic redirect on success

### Items Browser Form (`/items`)
**Location**: `Components/Pages/Items.razor`
**Model**: `Models/ItemsQueryModel.cs`

**Fields**:
- Collection Name (required, min length 1, default: "articles")
- Item Limit (range 1-100, default: 10)

**Features**:
- Validated collection name input
- Numeric limit with range validation
- Dynamic results display
- Error handling with user feedback

## Common Validation Patterns

### Required Fields
```csharp
[Required(ErrorMessage = "Field is required")]
public string FieldName { get; set; } = string.Empty;
```

### Email Validation
```csharp
[EmailAddress(ErrorMessage = "Invalid email format")]
public string Email { get; set; } = string.Empty;
```

### Numeric Range
```csharp
[Range(1, 100, ErrorMessage = "Value must be between 1 and 100")]
public int Number { get; set; }
```

### String Length
```csharp
[MinLength(5, ErrorMessage = "Minimum 5 characters")]
[MaxLength(100, ErrorMessage = "Maximum 100 characters")]
public string Text { get; set; } = string.Empty;
```

## Testing Forms

### Manual Testing Steps
1. **Empty Form Submission**: Try submitting without filling fields
   - Expected: Validation error messages appear
   
2. **Invalid Email**: Enter invalid email format
   - Expected: "Invalid email format" message
   
3. **Out of Range Values**: Enter values outside valid ranges
   - Expected: Range validation error messages
   
4. **Valid Submission**: Fill all fields correctly
   - Expected: Form submits successfully

### Validation Behavior
- **Client-side**: Validates on form submission (OnValidSubmit)
- **Server-side**: Can be added in API endpoints
- **Real-time**: Can be added with `OnParametersSet` or custom triggers

## Future Enhancements

### Potential Improvements
1. **Real-time Validation**: Validate on blur or value change
2. **Custom Validators**: Create custom validation attributes
3. **Async Validation**: Validate against API (e.g., email uniqueness)
4. **Form State Management**: Save draft state
5. **Multi-step Forms**: Wizard-style forms with validation per step

### Example: Real-time Validation
```razor
<InputText @bind-Value="model.Email" 
           @bind-Value:event="oninput"
           @onblur="ValidateEmail" />
```

## Troubleshooting

### Issue: Validation Messages Not Showing
**Solution**: Ensure `<DataAnnotationsValidator />` is inside `<EditForm>`

### Issue: Form Submits Without Validation
**Solution**: Use `OnValidSubmit` instead of `OnSubmit`

### Issue: Model Values Are Null/Empty
**Solution**: Initialize model in `OnInitialized()` with default values

### Issue: UI Not Updating After Async Operations
**Solution**: Call `StateHasChanged()` after state changes

## References
- [Blazor Forms Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/)
- [Data Annotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)
- [EditForm Component](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.forms.editform)
