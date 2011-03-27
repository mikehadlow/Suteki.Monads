type Identity<'a> = Identity of 'a

let getValue (a : Identity<'a>) = match a with Identity x -> x
let mreturn x = Identity x
let bind (a : Identity<'a>) (f : 'a -> Identity<'b>) = f (getValue a)

type IdentityBuilder() =
    member x.Bind(a, f) = bind a f
    member x.Return(a) = mreturn a

let identity = new IdentityBuilder()
let result = identity {
    let! a = Identity 4
    let! b = Identity 3
    return a + b
    }

printfn "result = %A" (getValue result)