using System;
using System.ServiceModel;
using PsGet.Helper.Serializables;
using System.Collections.Generic;

namespace PsGet.Helper {
    [ServiceContract(Namespace="http://ns.psget.org/helper", CallbackContract=typeof(INuGetClient))]
    public interface INuGetShim {
        [OperationContract(IsOneWay = true)]
        void Install(string id, Version version, string source, string destination);

        [OperationContract]
        ICollection<Package> GetPackages(string source, string filter, bool allVersions);

        [OperationContract]
        void Shutdown();
    }

    [ServiceContract(Namespace="http://ns.psget.org/helper/client")]
    public interface INuGetClient {
        [OperationContract(IsOneWay = true)]
        void ReportProgress(ProgressRecord record);

        [OperationContract(IsOneWay = true)]
        void Completed();
    }
}
