using CacheServer.Data;
using CacheServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text;
using System.Text.Json;

var cache = new MemoryCache(new MemoryCacheOptions());
var db = new AppDbContext();
var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:5000/");
listener.Start();
Console.WriteLine("server qalxdi");

while (true)
{
    var ctx = await listener.GetContextAsync();
    _ = Handle(ctx);
}

async Task Handle(HttpListenerContext ctx)
{
    try
    {
        var req = ctx.Request;
        var path = req.Url.AbsolutePath.Trim('/').Split('/');

        if (path[0] != "products")
        {
            Reply(ctx, 404, "yox");
            return;
        }

        using var db = new AppDbContext();
        var svc = new CacheService(db, cache);

        int? id = path.Length > 1 && int.TryParse(path[1], out int x) ? x : null;

        if (req.HttpMethod == "GET")
        {
            if (id == null)
                Reply(ctx, 200, await svc.GetAll());
            else
            {
                var p = await svc.Get(id.Value);
                Reply(ctx, p == null ? 404 : 200, p ?? (object)"yoxdur");
            }
        }
        else if (req.HttpMethod == "POST")
        {
            var p = JsonSerializer.Deserialize<Product>(new StreamReader(req.InputStream).ReadToEnd(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Reply(ctx, await svc.Add(p) ? 201 : 400, "ok");
        }
        else if (req.HttpMethod == "PUT")
        {
            var p = JsonSerializer.Deserialize<Product>(new StreamReader(req.InputStream).ReadToEnd(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Reply(ctx, await svc.Update(p) ? 200 : 404, "ok");
        }
        else if (req.HttpMethod == "DELETE")
        {
            if (id == null) { Reply(ctx, 400, "id lazim"); return; }
            Reply(ctx, await svc.Delete(id.Value) ? 200 : 404, "ok");
        }
        else Reply(ctx, 405, "yox");
    }
    catch (Exception ex)
    {
        Reply(ctx, 500, ex.Message);
    }
}

void Reply(HttpListenerContext ctx, int code, object data)
{
    var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    ctx.Response.StatusCode = code;
    ctx.Response.ContentType = "application/json";
    ctx.Response.OutputStream.Write(bytes);
    ctx.Response.Close();
}