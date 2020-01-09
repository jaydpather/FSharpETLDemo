module StateChecker

open GlobalTypes

let checkState nextFunction state = 
    match state with
    | Success s -> nextFunction s
    | Failure f -> Failure f

