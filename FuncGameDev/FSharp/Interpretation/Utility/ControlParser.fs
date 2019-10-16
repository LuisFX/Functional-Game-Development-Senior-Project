namespace Interpretation.Utility

    open System.Configuration
    open System

    module controlSettings = 
        let read (key : string) = 
            let controlSettings = ConfigurationManager.AppSettings
            let value = controlSettings.Item(key)
            if String.IsNullOrEmpty(value)
                then failwith "Key {0} is invalid" key
            value

