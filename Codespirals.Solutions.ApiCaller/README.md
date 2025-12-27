# Codespirals Api Caller

This is a small solution to easily call external web APIs.

To accomplish this, it employs dependency injection to add the `ApiCallerFactory` to our service collection while building our app:

<sub>Program.cs (or similar):</sub>

    services.AddApiCallerService(ServiceLifetime.Scoped)

After this service is added, we can inject the api caller factory it into our other classes, where we use it to instances of the `ApiCaller`

<sub>Another class:</sub>

    public class ApiExampleService(IApiCallerFactory apiCallerFactory)
    {
        var apiCaller1 = apiCallerFactory.InitializeApiCaller("https://my-api.com");
    }

Doing it this way allows us to easily create multiple api callers for different API urls.

Now that we have an `ApiCaller` we can make use of it.

Because calls to an API are largely similar, we have built in the most common methods to be called into the Caller.

<sub>Somewhere within the other class:</sub>

    var result = await apiCaller1.Get("xxx");
    var result = await apiCaller1.Get<T>("xxx");

Where `T` is the expected result class, if there is one, and the `xxx` represent the slug of the api endpoint we're calling.

The api caller takes the given info, executes the api call and returns the result to us in the form of an `ApiResult`.

This result contains the following:

<sub>An ApiResult:</sub>

    bool Success
    HttpStatusCode StatusCode
    string? ErrorCode
    string Error
    T? Data

Most important in this is the `Success` boolean - this gives us a very quick and easy way to check if the call was successful.

I decided to implement the Result Pattern here to get a more unified response. I don't have to guess what was actually returned, I can just check `.Success` and take it from there.

## Advanced stuff

### Search & Pagination

There are two variations on `ApiResult` to handle pagination, namely `ApiFilteredListResult` and `ApiSearchResult` - the only difference between them is that search has an additional property for query.

They implement `IPagination`, which contains all info needed to paginate a result.

`ApiCaller` has a method prepared to make using these relatively easy:

    var result = await apiCaller1.GetPaginated<TData, TResponse, TFilterParameters>(parameters, "XXX";
    var result = await apiCaller1.Search<TData, TResponse, TFilterParameters>(parameters, "XXX";

The only minorly tricky part is that to make these methods work, the return value from the API calls has to implement `IPagination` as well - which is represented and enforced by `TResponse` in these methods.

How the methods work in the backend is that they turn the search parameter object into query parameters and send a get request.

However, as soon as [Http Query](https://httpwg.org/http-extensions/draft-ietf-httpbis-safe-method-w-body.html) is in more wide use, I'll probably rework these methods.

### Build-a-call

This library allows customizing your own API calls. To do this we use `BeginCustomApiCall()`

    apiCaller1.BeginCustomApiCall()

This returns a `HttpRequestBuilder`, from which we can chain further customizations.

When we're done we can then fire off the api call with `.Send()`

<sub>Example of builder chaining:</sub>

    apiCaller1.BeginCustomApiCall().WithEndpoint("xxx").WithBody<T>(foo).Send<TReturn>(HttpMethod.Post)

