module Messages

let dividingLine = "\n---------------------------------------\n"

let renderTable message = 
    sprintf "%sNext action on: %s%s" dividingLine message dividingLine

let renderFooter message =
    sprintf "%s%s" message dividingLine
        
let renderHeader message =
    sprintf "%s%s" dividingLine message 
