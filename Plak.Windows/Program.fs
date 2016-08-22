open System
open System.IO
open System.Net
open System.Reflection
open System.Text
open System.Text.RegularExpressions
open System.Threading
open System.Windows.Forms

[<assembly: AssemblyTitle("Plak.Windows")>]
[<assembly: AssemblyDescription("Copypaste between Bash on Ubuntu on Windows and Windows")>]
[<assembly: AssemblyConfiguration("")>]
[<assembly: AssemblyCompany("Sander Dijkhuis")>]
[<assembly: AssemblyProduct("Plak.Windows")>]
[<assembly: AssemblyCopyright("Copyright © 2016 Sander Dijkhuis")>]
[<assembly: AssemblyTrademark("")>]
[<assembly: AssemblyCulture("")>]
[<assembly: AssemblyVersion("1.0.0.0")>]
[<assembly: AssemblyFileVersion("1.0.0.0")>]
()

let clipboard() =
    if Clipboard.ContainsText(TextDataFormat.UnicodeText)
    then Some(Clipboard.GetText(TextDataFormat.UnicodeText))
    else None

let respond (ctx : HttpListenerContext) (code, (content : string)) =
    let bytes = Encoding.UTF8.GetBytes(content)
    use output = ctx.Response.OutputStream

    ctx.Response.StatusCode <- code
    ctx.Response.ContentType <- "text/plain; charset=utf-8"
    output.Write(bytes, 0, bytes.Length)

let readContent (input : Stream) =
    use reader = new StreamReader(input)
    let content = reader.ReadToEnd()
    Regex.Replace(content, @"\r?\n", Environment.NewLine)

let responseForGet content =
    match content with
    | Some(s) -> 200, s
    | None -> 404, "no content"

let handlePut (req : HttpListenerRequest) =
    match readContent req.InputStream with
    | "" -> Clipboard.Clear()
    | s -> Clipboard.SetText(s, TextDataFormat.UnicodeText)
    200, ""

[<EntryPoint>]
[<STAThread>]
let main argv =
    let port = match argv with
               | [|p|] -> try System.Int32.Parse p
                          with | :? FormatException -> failwith "Argument must be a number"
               | [||] -> 9001
               | _ -> failwith "Must have zero or one argument (port number)"
    let listener = new HttpListener()

    listener.Prefixes.Add(sprintf "http://localhost:%d/clipboard/" port)

    try listener.Start()
    with | :? HttpListenerException as e -> failwith e.Message

    while true do
        let ctx = listener.GetContext()
        match ctx.Request.HttpMethod with
        | "GET" -> responseForGet (clipboard())
        | "PUT" -> handlePut ctx.Request
        | _ -> 400, "only works with GET or PUT"
        |> respond ctx

    ignore (Console.ReadLine())

    0
