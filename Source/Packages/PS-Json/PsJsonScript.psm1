$acc = [Type]::GetType("System.Management.Automation.TypeAccelerators");
$acc::Add("json", [Newtonsoft.Json.Linq.JToken]);