#r "nuget: Microsoft.Data.Sqlite"

open Microsoft.Data.Sqlite
open System.IO
open System

let args = fsi.CommandLineArgs |> Array.tail

let mslPath =
    match args.Length with
    | 0 -> failwith "No msl path is given"
    | _ -> args |> Array.head

let mslDirectory = (Directory.GetParent mslPath).FullName

let getJobIndices dbPath =
    use connection =
        new SqliteConnection(sprintf "Data Source=%s" dbPath)

    use command =
        new SqliteCommand("select Id from JobModels order by Id DESC", connection)

    connection.Open()

    let rec consumeReader list (reader: SqliteDataReader) =
        if reader.Read() then
            let id = reader.GetInt32(0)
            consumeReader (id :: list) reader
        else
            list

    command.ExecuteReader() |> consumeReader []

let getResultFiles jobFolder =
    let statePath = sprintf "%s/run.mcs" jobFolder
    let preStatePath = sprintf "%s/prerun.mcs" jobFolder
    let stdoutPath = sprintf "%s/stdout.log" jobFolder
    (statePath, preStatePath, stdoutPath)

let loadBytesOrGetDbNull path =
    if File.Exists path then
        File.ReadAllBytes path :> obj
    else
        DBNull.Value :> obj

let collectData dbPath indices =
    use connection =
        new SqliteConnection(sprintf "Data Source=%s" dbPath)

    use writeCmd =
        new SqliteCommand(
            "update JobResultData set ResultState = $state, PreRunState = $prestate, Stdout = $stdout where JobModelId = $id",
            connection
        )

    connection.Open()

    for jobId in indices do
        let jobFolder = sprintf "%s/Job%05i" mslDirectory jobId

        if Directory.Exists jobFolder then
            printf "Collecting job ... %05d\r" jobId

            let (statePath, prePath, stdoutPath) = getResultFiles jobFolder
            let state = loadBytesOrGetDbNull statePath
            let preState = loadBytesOrGetDbNull prePath
            let stdout = loadBytesOrGetDbNull stdoutPath

            writeCmd.Parameters.AddWithValue("$id", jobId)
            |> ignore

            writeCmd.Parameters.AddWithValue("$state", state)
            |> ignore

            writeCmd.Parameters.AddWithValue("$prestate", preState)
            |> ignore

            writeCmd.Parameters.AddWithValue("$stdout", stdout)
            |> ignore

            if writeCmd.ExecuteNonQuery() <> 1 then
                failwith "Error while writing data"

            writeCmd.Parameters.Clear()

    printfn "\nDone!"

let printStartInfo path indices =
    printfn "Collecting %i jobs for: %s" (List.length indices) path

let jobIndices = getJobIndices mslPath
printStartInfo mslPath jobIndices
collectData mslPath jobIndices
