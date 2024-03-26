# BlazorIconify

![BlazorIconify Logo](https://github.com/bub-bl/Iconify/assets/66354328/ec3b9c87-72a4-4cd3-8312-389a3ad89052)

BlazorIconify is a C# wrapper designed to simplify the retrieval and caching of icons from the [Iconify](https://iconify.design/) API. With its intuitive interface, BlazorIconify streamlines the process of fetching icons and caching them for efficient reuse in your applications.

## Features

- **Effortless Icon Retrieval**: Access icons from the Iconify API with ease, simplifying your development workflow.
- **Built-in Caching Mechanism**: Save network bandwidth and load times by caching icons in the local storage for swift access when needed.
- **Seamless Integration**: Integrate BlazorIconify seamlessly into your projects to enhance their visual appeal and functionality.
- **HTML Usage Example**: Quickly incorporate icons into your HTML markup using the provided code snippet.

## Getting Started

To get started with BlazorIconify, simply follow these steps:

1. Install the BlazorIconify package via NuGet Package Manager or .NET CLI:

    ```
    dotnet add package BlazorIconify
    ```

2. Register the Iconify service in your application:

    ```csharp
    builder.Services.AddIconify();
    ```

3. Begin using icons in your HTML:

    ```html
    <Iconify Icon="material-symbols:arrow-left-alt-rounded" style="color: white"></Iconify>
    ```

## Contributing

We welcome contributions from the community to make BlazorIconify even better! Whether it's through bug reports, feature suggestions, or code contributions, your input is highly valued. To contribute, please follow these guidelines:

1. Fork the repository and clone it to your local machine.
2. Create a new branch for your feature or bug fix.
3. Make your changes and ensure that all tests pass.
4. Commit your changes with descriptive commit messages.
5. Push your changes to your fork and submit a pull request to the `main` branch of the BlazorIconify repository.

## License

BlazorIconify is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.
