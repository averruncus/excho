namespace Excho

open System

[<AutoOpen>]
module CoreExts =
  let tryDo (f : 't -> 'r option) t =
    f t
  let withDefault d r =
    match r with
    | Some r -> r
    | _ -> d
  let (?|) (t : 't option) d = match t with | Some t -> t | _ -> d
  let (?||) (t : 't Nullable) d = t.GetValueOrDefault(d)
  let (?|||) t d = if t <> null then t else d