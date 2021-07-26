namespace Fable.Form.Simple.MaterialUI

[<RequireQualifiedAccess>]
module Form =
    module View =

        open Fable.Form
        open Feliz
        open Feliz.MaterialUI
        open Fable.Form.Simple
        open Fable.Form.Simple.Form.View

        let form (config: FormConfig<'Msg>) =
            Mui.formControl [
                prop.children [ yield! config.Fields ]
                prop.onSubmit (fun  ev ->

                    ev.stopPropagation()
                    ev.preventDefault()

                    config.OnSubmit
                    |> Option.map config.Dispatch
                    |> Option.defaultWith ignore
                    )
            ]

        let htmlViewConfig<'Msg> : CustomConfig<'Msg> =
            {
                Form = form
                TextField = failwith "Not implemented yet"
                PasswordField = failwith "Not implemented yet"
                EmailField = failwith "Not implemented yet"
                TextAreaField = failwith "Not implemented yet"
                CheckboxField = failwith "Not implemented yet"
                RadioField = failwith "Not implemented yet"
                SelectField = failwith "Not implemented yet"
                Group = failwith "Not implemented yet"
                Section = failwith "Not implemented yet"
                FormList = failwith "Not implemented yet"
                FormListItem = failwith "Not implemented yet"
            }

        // Function which will be called by the consumer to render the form
        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config