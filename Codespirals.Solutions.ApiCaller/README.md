# Codespirals Api Caller

This is a small solution to easily call external web APIs.

## ApiCallerFactory

To accomplish this, it employs dependency injection to add the `ApiCallerFactory` to our service collection while building our app:

**In Program.cs (or similar):**

    services.AddApiCallerService()

## Creating ApiCallers

After this service is added, we can inject it into our other classes, where we use it to create instances of the `ApiCaller` class.

**How to use it in your class:**

    public class ApiExampleService(IApiCallerFactory apiCallerFactory)
    {
        var apiCaller1 = apiCallerFactory.InitializeApiCaller("https://my-api.com");
        var apiCaller2 = apiCallerFactory.InitializeApiCaller("https://my-other-api.com");
    }

As you can see, doing it this way allows us to easily create multiple api callers for different API urls.

Now that we have an `ApiCaller` we can make use of it.

Because calls to an API are largely similar, we have built in the most common methods to be called into the Caller.

**Somewhere within the other class:**

    var result = await apiCaller1.Get("xxx");
    var result = await apiCaller1.Get<T>("xxx");

Where `T` is the expected result class, if there is one, and the `xxx` represent the slug of the api endpoint we're calling.

The api caller takes the given info, executes the api call and returns the result to us in the form of an `ApiResult`.

This result contains the following:

**An ApiResult:**

    bool Success
    HttpStatusCode StatusCode
    string? ErrorCode
    string Error
    T? Data

Most important in this is the `Success` boolean - this gives us a very quick and easy way to check if the call was successful.

I decided to implement the Result Pattern here to get a more unified response. I don't have to guess what was actually returned, I can just check `.Success` and take it from there.

## Advanced stuff

### Search & Pagination

The advantage of pagination, should you be able to use it, is that your backend can do all the work of formatting and caching the data and only sending the requested page back, instead of all possible search results.

My solution for search and pagination is a variation on the above `ApiResult` - the predictably named `PaginatedApiResult`.

It implements my base interface `IPagination`, which contains all info needed to paginate a result.

`ApiCaller` has a method prepared to make using these relatively easy:

    var result = await apiCaller1.GetPaginated<TData, TResponse, TFilterParameters>("xxx", parameters);
    var result = await apiCaller1.Search<TData, TResponse, TFilterParameters>("xxx", parameters);

The difference between the two being that "search" expects an additonal query string in the parameters.

The only minorly tricky part is that to make these methods work, the return value from the API calls has to implement `IPagination` as well - which is represented and enforced by `TResponse` in these methods.

How the methods work in the backend is that they turn the search parameter object into query parameters and send a get request.

**Alternatively**

Since June 2026 a new HTTP method called "Query" exists - I have implemented, but I'm keeping the other methods for now as "query" isn't implemented everywhere yet.

### Build-a-call

This library allows customizing your own API calls. To do this we use `BeginCustomApiCall()`

    apiCaller1.BeginCustomApiCall()

This returns a `HttpRequestBuilder`, from which we can chain further customizations.

When we're done we can then fire off the api call with `.Send()`

**Example of builder chaining:**

    apiCaller1.BeginCustomApiCall().WithEndpoint("xxx").WithBody<T>(foo).Send<TReturn>(HttpMethod.Post)

