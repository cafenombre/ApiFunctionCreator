using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiFunctionCreator
{
    class response
    {
        public List<attribute> attributes;
    }
    class request
    {
        public List<attribute> attributes;
    }

    class addedClasses
    {
        public string className;
        public List<attribute> attributes;

        public addedClasses(string className, List<attribute> attributes)
        {
            this.className = className;
            this.attributes = attributes;
        }
    }
    class attribute
    {
        public string csType;
        public string tsType;
        public string name;

        public attribute(string csType, string tsType, string name)
        {
            this.csType = csType;
            this.tsType = tsType;
            this.name = name;
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            /* Note importante, le path doit fini par un / */


            /* Si le projet est sur expert, le client doit être le SmartBuisness.web, si il est sur modeler, ce sera le Librairy_API */
            string client = "D:/Techform/SmartConfigurator/V6.0.1/Applications/GroupeFPEE_FPEE_PORTAIL/API_Librairy/";//"C:/Users/tglotin/Desktop/ApiFolder/client/modeler/"; 

            //"D:/Techform/SmartConfigurator/V6.0.1/Applications/GroupeFPEE_FPEE_PORTAIL/SmartBusiness.Web/";
            //string client = "D:/CADIOU/CADIOU_Portail/Solution/SmartBusiness.Web/";

            string server = "D:/Techform/SmartConfigurator/V6.0.1/Applications/DemoBS_601_T0_PORTAIL/SmartBusiness.Web/";// "C:/Users/tglotin/Desktop/ApiFolder/server/";




            List<addedClasses> classes = new List<addedClasses>();

            #region classes to add
            /*classes.Add(new addedClasses(
                "APICLASS", new List<attribute> {
                    new attribute("string", "string", "apiname"),
                    new attribute("int", "number", "apiage")
                }));

            classes.Add(new addedClasses(
                "APICULTEUR", new List<attribute> {
                    new attribute("string", "string", "stringerino"),
                    new attribute("List<APICLASS>", "Array<APICLASS>", "Apicassos")
                }));*/
            #endregion

            /* !!! Attention de bien placer les placeholders /*APICreatorPlaceholder*/
            /* Dans les fichiers : IService.cs, Service.ts (s'il existe) et ViewModel (si renseigné) à l'endroit ou vous voulez les fonctione écrites */


            /* si le nom view Model est  laissé vide, il est ignoré et rien ne sera fait coté VM */

            /* --- EXEMPLE EXPERT --- */
            /*
            addApiMethod(classes, 
                new response()
            {
                attributes = new List<attribute> {
                    new attribute("bool", "boolean", "Success"),
                    new attribute("string", "string", "message")
                }
            }, 
                new request()
            {
                attributes = new List<attribute> {
                    new attribute("string", "string", "guid"),
                    new attribute("int", "number", "index"),
                    new attribute("int", "number", "height"),
                    new attribute("int", "number", "width"),
                    new attribute("string", "string", "color"),
                }
            }, "SetPillar", client, server, nomViewModel: "FormulaireProduitViewModel.ts", subFolderViewModel : "", isModeler: false);
            */

            /* --- EXEMPLE MODELER --- */
            addApiMethod(classes,
                new response()
                {
                    attributes = new List<attribute> {
                    new attribute("bool", "boolean", "Success"),
                    new attribute("string", "string", "message")
                }
                },
                new request()
                {
                    attributes = new List<attribute> {
                    new attribute("string", "string", "guid"),
                    new attribute("int", "number", "index"),
                    new attribute("int", "number", "width")
                }
                }, "SetLeafWidth", client, server, nomViewModel: "", subFolderViewModel: "", isModeler: true);







        }

        public static void EditorialResponse(string directory, string fileName, string placeholder, string replacement)
        {
            StreamReader reader = new StreamReader(directory + fileName);
            string input = reader.ReadToEnd();

            string output = input.Replace(placeholder, replacement);
            reader.Close();

            File.WriteAllText(directory + fileName, output);
        }


        public static void addApiMethod(List<addedClasses> addedClasses, response response, request request, string nomMethod, string clientPath, string serverPath, string nomViewModel, string subFolderViewModel, bool isModeler)
        {

            string nomRequest = nomMethod + "Request";
            string nomResponse = nomMethod + "Response";

            string nameSpaceServer = "SmartBusiness.Web.Models";
            string nameSpaceClient = "API_Librairy.Models";

            /* --- Coté serveur --- */

            //Créer modèles (ajouter folder s'il n'existe pas) créer request, response and classes

            #region gestion des dossiers
            /* Dossiers serveurs (créer s'ils n'éxistent pas) */

            string mainServeurPath = serverPath + "Models/";
            string ClassesServerPath = mainServeurPath + "Classes/";
            string RequestServerPath = mainServeurPath + "Requests/";
            string ResponseServerPath = mainServeurPath + "Responses/";

            System.IO.Directory.CreateDirectory(mainServeurPath);
            System.IO.Directory.CreateDirectory(ClassesServerPath);
            System.IO.Directory.CreateDirectory(ResponseServerPath);
            System.IO.Directory.CreateDirectory(RequestServerPath);

            /* Dossiers client (créer s'ils n'éxistent pas) */

            string rootClientPath = isModeler ? clientPath : clientPath + "Proxy/";
            string mainClientPath = rootClientPath + "Models/";
            string ClassesClientPath = mainClientPath + "Classes/";
            string RequestClientPath = mainClientPath + "Requests/";
            string ResponseClientPath = mainClientPath + "Responses/";

            System.IO.Directory.CreateDirectory(rootClientPath);
            System.IO.Directory.CreateDirectory(mainClientPath);
            System.IO.Directory.CreateDirectory(ClassesClientPath);
            System.IO.Directory.CreateDirectory(ResponseClientPath);
            System.IO.Directory.CreateDirectory(RequestClientPath);

            string VMPath = "";
            if (nomViewModel != "")
            {
                VMPath = clientPath + "ViewModels/" + subFolderViewModel;
                System.IO.Directory.CreateDirectory(VMPath);
            }


            #endregion

            #region classes à ajouter

            foreach (addedClasses c in addedClasses)
            {
                string csName = c.className + ".cs";
                string tsName = c.className + ".ts";

                #region server_side
                string classContentCs = "namespace " + nameSpaceServer + " {";
                classContentCs += "\n   public class " + c.className + "{";

                /* Ajout des attributs */
                foreach (attribute a in c.attributes)
                {
                    classContentCs += attributeToCs(a);
                }

                classContentCs += "\n   }";
                classContentCs += "\n}";

                File.WriteAllText(ClassesServerPath + csName, classContentCs);

                #endregion

                #region client_side
                if (isModeler)
                {
                    File.WriteAllText(ClassesClientPath + csName, classContentCs);
                    //Replace the namespace
                    EditorialResponse(ClassesClientPath, csName, nameSpaceServer, nameSpaceClient);
                }

                else
                {
                    string classContentTs = "module SmartBusiness.Web.Proxy.Models {";
                    classContentTs += "\n   export class " + c.className + "{";

                    /* Ajout des attributs */
                    foreach (attribute a in c.attributes)
                    {
                        classContentTs += attributeToTs(a);
                    }

                    classContentTs += "\n   }";
                    classContentTs += "\n}";

                    File.WriteAllText(ClassesClientPath + tsName, classContentTs);
                }

                #endregion

            }
            #endregion

            #region request

            string nomRequestCs = nomRequest + ".cs";
            string nomRequestTs = nomRequest + ".ts";

            #region server_side
            string requestContentCs = "namespace " + nameSpaceServer + " {";
            requestContentCs += "\n   public class " + nomRequest + "{";

            /* Ajout des attributs */
            foreach (attribute a in request.attributes)
            {
                requestContentCs += attributeToCs(a);
            }

            requestContentCs += "\n   }";
            requestContentCs += "\n}";

            File.WriteAllText(RequestServerPath + nomRequestCs, requestContentCs);

            #endregion

            #region client_side

            if (isModeler)
            {
                File.WriteAllText(RequestClientPath + nomRequestCs, requestContentCs);
                EditorialResponse(RequestClientPath, nomRequestCs, nameSpaceServer, nameSpaceClient);
            }

            else
            {
                string requestContentTs = "module SmartBusiness.Web.Proxy.Models {";
                requestContentTs += "\n   export class " + nomRequest + "{";

                /* Ajout des attributs */
                foreach (attribute a in request.attributes)
                {
                    requestContentTs += attributeToTs(a);
                }

                requestContentTs += "\n   }";
                requestContentTs += "\n}";

                File.WriteAllText(RequestClientPath + nomRequestTs, requestContentTs);
            }
            #endregion

            #endregion

            #region response

            string nomResponseCs = nomResponse + ".cs";
            string nomResponseTs = nomResponse + ".ts";

            #region server_side
            string responseContentCs = "namespace " + nameSpaceServer + " {";
            responseContentCs += "\n   public class " + nomResponse + "{";

            /* Ajout des attributs */
            foreach (attribute a in response.attributes)
            {
                responseContentCs += attributeToCs(a);
            }

            responseContentCs += "\n   }";
            responseContentCs += "\n}";

            File.WriteAllText(ResponseServerPath + nomResponseCs, responseContentCs);

            #endregion

            #region client_side
            if (isModeler)
            {
                File.WriteAllText(ResponseClientPath + nomResponseCs, responseContentCs);
                EditorialResponse(ResponseClientPath, nomResponseCs, nameSpaceServer, nameSpaceClient);
            }

            else
            {
                string responseContentTs = "module SmartBusiness.Web.Proxy.Models {";
                responseContentTs += "\n   export class " + nomResponse + "{";

                /* Ajout des attributs */
                foreach (attribute a in response.attributes)
                {
                    responseContentTs += attributeToTs(a);
                }

                responseContentTs += "\n   }";
                responseContentTs += "\n}";

                File.WriteAllText(ResponseClientPath + nomResponseTs, responseContentTs);
            }
            #endregion
            #endregion

            #region modification fichers serveur

            #region Iservice.cs
            /* --- IService.cs --- */
            string IServiceFile = "IService.cs";

            /* -- CE PLACEHOLDER DOIT IMPERATIVEMENT ÊTRE PLACÉ DANS LE FICHIER A L'ENDROIT ATTENDU DE LA FONCTION -- */
            string placeholderIS = "/*APICreatorPlaceholder*/";

            string contentIService = placeholderIS;

            /* Ajout de l'operation contract */
            contentIService += "\n        [OperationContract]" +
                "\n        [WebInvoke(Method = \"POST\"," +
                "\n            UriTemplate = \"" + nomMethod + "\"," +
                "\n            RequestFormat = WebMessageFormat.Json," +
                "\n            ResponseFormat = WebMessageFormat.Json," +
                "\n            BodyStyle = WebMessageBodyStyle.Bare)]" +
                "\n        " + nomResponse + " " + nomMethod + "(" + nomRequest + " request);\n";

            EditorialResponse(serverPath, IServiceFile, placeholderIS, contentIService);
            #endregion

            #region Service.svc.cs
            /* --- Service.svc.cs --- */
            string ServiceSvcFile = "Service.svc.cs";
            string placeholderSvc = "#region IService Members";

            string contentServiceSvc = placeholderSvc;

            /* Ajout de la fonction API */
            contentServiceSvc += "\n" +
                "\n        public " + nomResponse + " " + nomMethod + "(" + nomRequest + " request)" +
                "\n        {" +
                "\n            string message = \"Communication etablie avec paramètres : ";

            foreach (attribute a in request.attributes)
            {
                /* name = " + request.name + "  */
                contentServiceSvc += " " + a.name + " = \" + request." + a.name + " + \"";
            }
            contentServiceSvc += "\";" +
                "\n            " +
                "\n            /* Content of the API Funcion */" +
                "\n            " +
                "\n            var response = new " + nomResponse + "()" +
                "\n            {";
            /*Initialisation de la réponse à renvoyer au client pour chaque parametre*/
            foreach (attribute attribute in response.attributes)
            {
                contentServiceSvc += "\n                " + attribute.name + " = null ,";
            }

            contentServiceSvc = contentServiceSvc.Remove(contentServiceSvc.Length - 1);
            contentServiceSvc += "\n            };" +
                "\n            " +
                "\n            return response;" +
                "\n        }" +
                "\n";

            EditorialResponse(serverPath, ServiceSvcFile, placeholderSvc, contentServiceSvc);
            #endregion

            #endregion

            #region modification fichiers client

            if (isModeler)
            {
                #region Librairy.cs

                /* --- Librairy.cs --- */
                string LibFile = "Librairy.cs";

                /* -- CE PLACEHOLDER DOIT IMPERATIVEMENT ÊTRE PLACÉ DANS LE FICHIER A L'ENDROIT ATTENDU DE LA FONCTION -- */
                string placeholderLib = "/*APICreatorPlaceholder*/";

                string contentLib = placeholderIS;

                contentLib += "\n" +
                "\n        public static " + nomResponse + " " + nomMethod + "(";

                foreach (attribute a in request.attributes)
                {
                    contentLib += a.csType + " " + a.name + ", ";
                }
                contentLib = contentLib.Remove(contentLib.Length - 2);
                contentLib += ")" +
                "\n        {" +
                "\n            if (!String.IsNullOrEmpty(APIurl) && (HttpContext.Current != null)){" +
                "\n                string methodurl = APIurl + \"/service.svc/" + nomMethod + "\";" +
                "\n                " + nomRequest + " request = new " + nomRequest + "{";

                foreach (attribute a in request.attributes)
                {
                    contentLib += "\n                   " + a.name + " = " + a.name + " ,";
                }
                contentLib = contentLib.Remove(contentLib.Length - 1);
                contentLib += "\n                   };" +
                    "\n            " +
                    "\n                " +
                    "\n                " + nomResponse + " response = SmartWebREST.Json.JsonHelpers.JsonRequest<" + nomRequest + ", " + nomResponse + ">(methodurl, request, true);" +
                    "\n                " +
                    "\n                return response;" +
                    "\n            }" +
                    "\n            return null;" +
                    "\n        }" +
                    "\n";

                EditorialResponse(clientPath, LibFile, placeholderLib, contentLib);

                #endregion
            }
            else
            {
                #region Service.ts 
                /* Le service.ts doit-être créé s'il n'existe pas */
                string ServiceTsFile = "Service.ts";
                string ServiceTsPath = rootClientPath + ServiceTsFile;

                if (!File.Exists(ServiceTsPath))
                    createServiceTs(ServiceTsPath);

                /* Ajout de l'appel dans le fichier service */
                string placeholderServiceTs = "/*APICreatorPlaceholder*/";
                string contentServiceTs = placeholderServiceTs;

                contentServiceTs += "\n        public static " + nomMethod + "(path: string, request: Models." + nomRequest + ", callBack: (response: Models." + nomResponse + ") => void, callError?: (e: string) => void): void {" +
                    "\n            var webMethod: string =  path + \"/Service.svc/" + nomMethod + "\";" +
                    "\n            var parameters = request" +
                    "\n            SmartFrame.JS.Ajax.Request(webMethod, parameters, (msg) => {" +
                    "\n                callBack(msg);" +
                    "\n            }, true, (e) => {" +
                    "\n                if(callError) {" +
                    "\n                    callError(e);" +
                    "\n                }" +
                    "\n            });" +
                    "\n        }\n";

                EditorialResponse(rootClientPath, ServiceTsFile, placeholderServiceTs, contentServiceTs);
                #endregion

                #region ViewModel
                /* -- If the viewModel name is given, we do it, otherwise ignore this part -- */
                if (nomViewModel != "")
                {
                    string VMFilePath = VMPath + nomViewModel;

                    string placeholderVm = "/*APICreatorPlaceholder*/";
                    string contentVm = placeholderVm;

                    contentVm += "\n        public " + nomMethod + "(){" +
                        "\n          /** URL API **/" +
                        "\n          /** Rentrez ici l'url de votre API (par le schema ou directement dans url_api **/" +
                        "\n          var url_api = this.Configuration()().API_TemplatePortail().API_UI().API_URL().toString()" +
                        "\n          /** **/" +
                        "\n          var request = new Proxy.Models." + nomRequest + "();" +
                        "\n";

                    foreach (attribute a in request.attributes)
                    {
                        contentVm += "\n          request." + a.name + " = null;";
                    }

                    contentVm += "\n" +
                        "\n          Proxy.Service." + nomMethod + "(url_api, request, (response) => {" +
                        "\n             console.log('methode api " + nomMethod + " créée avec succès')" +
                        "\n          }, (e) => {" +
                        "\n             console.log('erreur appel medthode " + nomMethod + "' + e);" +
                        "\n          });" +
                        "\n        }";

                    EditorialResponse(VMPath, nomViewModel, placeholderVm, contentVm);

                }


                #endregion
            }


            #endregion

        }

        public static string attributeToCs(attribute attr)
        {
            return "\n      public " + attr.csType + " " + attr.name + " { get; set; }";
        }

        public static string attributeToTs(attribute attr)
        {
            return "\n      public " + attr.name + ": " + attr.tsType + ";";
        }

        public static void createServiceTs(string ServiceTsPath)
        {
            string fileContent = "module SmartBusiness.Web.Proxy {";
            fileContent += "\n    export class Service {" +
                "\n" +
                "\n        /*APICreatorPlaceholder*/" +
                "\n" +
                "\n    }" +
                "\n}";

            byte[] contentEncoded = new UTF8Encoding(true).GetBytes(fileContent);
            FileStream fs = File.Create(ServiceTsPath);
            fs.Write(contentEncoded, 0, contentEncoded.Length);
            fs.Close();
        }
    }
}
