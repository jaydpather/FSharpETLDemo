module Constants

//todo: maybe have a Retry status so it's more transparent. (currently, just using New status for records that need to be re-tried)
type ImportStatusNames = {
    New:string;
    InProgress:string;
    Failed:string;
}

//todo: how to make these compile-time constants?
let ImportStatusNames = {
    New = "New";
    InProgress = "In Progress";
    Failed = "Failed"; 
}