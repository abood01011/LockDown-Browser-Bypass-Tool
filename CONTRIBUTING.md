# Contributing to LockDown Browser Bypass Tool

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing to this project.

## Code of Conduct

This project is intended for educational purposes. All contributors must:

- Write code responsibly and ethically
- Not encourage or facilitate academic dishonesty
- Respect others in discussions and pull requests
- Follow all applicable laws and regulations

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](../../issues)
2. Create a new issue with:
   - Clear title describing the bug
   - Steps to reproduce
   - Expected vs actual behavior
   - System information (Windows version, .NET version)
   - Screenshots if applicable

### Suggesting Features

1. Check [Issues](../../issues) for existing feature requests
2. Create a new issue with:
   - Clear description of the feature
   - Use case and benefits
   - Potential implementation approach

### Pull Requests

1. **Fork the repository**
   ```bash
   git clone https://github.com/yourusername/lockdown-browser-bypass.git
   cd lockdown-browser-bypass
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**
   - Follow the coding standards below
   - Add comments for complex logic
   - Update documentation if needed

4. **Test your changes**
   - Build the project successfully
   - Test all affected functionality
   - Verify no existing features are broken

5. **Commit your changes**
   ```bash
   git add .
   git commit -m "Add feature: brief description"
   ```

6. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **Create Pull Request**
   - Provide a clear description of changes
   - Reference any related issues
   - Wait for review and address feedback

## Coding Standards

### C# Style Guide

- Use 4 spaces for indentation (no tabs)
- Follow Microsoft C# naming conventions:
  - PascalCase for classes, methods, properties
  - camelCase for local variables, parameters
  - _camelCase for private fields
- Add XML documentation comments for public methods
- Keep methods focused and under 50 lines when possible

### Example:
```csharp
/// <summary>
/// Simulates a keyboard shortcut with the specified modifiers
/// </summary>
/// <param name="key">The key to press</param>
/// <param name="modifiers">Modifier keys (Ctrl, Shift, Alt)</param>
private void SimulateKeyPress(Keys key, KeyModifiers modifiers)
{
    // Implementation here
}
```

### Code Organization

- Keep related functionality in the same file
- Separate concerns (UI, business logic, Windows API calls)
- Use meaningful variable and method names
- Avoid magic numbers (use named constants)

### Error Handling

- Always use try-catch for potentially failing operations
- Log errors appropriately
- Provide user-friendly error messages
- Don't silently swallow exceptions

## Testing

While this project doesn't have automated tests yet, please manually test:

1. All hotkey combinations work correctly
2. Window switching functions as expected
3. System tray icon and menu work properly
4. Configuration loading and saving
5. Application exits cleanly

## Documentation

When adding features, update:

- README.md - if user-facing functionality changes
- BUILD.md - if build process changes
- Code comments - for complex logic
- Configuration examples - if new settings are added

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Questions?

Feel free to ask questions by:
- Creating an issue with the "question" label
- Starting a discussion in [Discussions](../../discussions)

## Recognition

Contributors will be recognized in the project README. Thank you for making this project better!
