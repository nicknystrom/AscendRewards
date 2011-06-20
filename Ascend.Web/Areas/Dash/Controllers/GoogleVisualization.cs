using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Dash.Controllers
{
    public class GoogleVisualizationTable
    {
        public GoogleVisualizationTable AddColumn<T>(string id, string label, string format)
        {
            if (null == Columns)
            {
                Columns = new List<GoogleVisualizationColumn>();
            }
            Columns.Add(new GoogleVisualizationColumn {
                Id = id,
                Label = label,
                Pattern = format,
                Type = GoogleVisualizationDataType.FromSystemType(typeof(T)),
            });
            return this;
        }

        public GoogleVisualizationTable AddRow(params object[] values)
        {
            return AddRow((IEnumerable<object>)values);
        }

        public GoogleVisualizationTable AddRow(IEnumerable<object> values)
        {
            if (null == Columns)
            {
                throw new InvalidOperationException("You must add columns to this Table before adding Rows.");
            }
            if (null == Rows)
            {
                Rows = new List<GoogleVisualizationRow>();
            }
            var r = new GoogleVisualizationRow();
            r.Cells = new List<GoogleVisualizationCell>();
            var i = 0;
            foreach (var v in values)
            {
                if (i >= Columns.Count)
                {
                    throw new IndexOutOfRangeException("There are more Cells in this Row than Columns in the Table.");
                }
                var c= new GoogleVisualizationCell { Value = v, };
                if (!String.IsNullOrEmpty(Columns[i].Pattern))
                {
                    c.Format = String.Format(Columns[i].Pattern, c.Value);
                }
                r.Cells.Add(c);
                i++;
            }
            Rows.Add(r);
            return this;
        }

        [JsonProperty("cols")] public IList<GoogleVisualizationColumn> Columns { get; set; }
        [JsonProperty("rows")] public IList<GoogleVisualizationRow> Rows { get; set; }
        [JsonProperty("p")] public IDictionary<string, string> Attributes { get; set; } 
    }

    public class GoogleVisualizationColumn
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("label")] public string Label { get; set; }
        [JsonProperty("pattern")] public string Pattern { get; set; }
        [JsonProperty("p")] public IDictionary<string, string> Attributes { get; set; } 
    }

    public static class GoogleVisualizationDataType
    {
        public static string FromSystemType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean: return Boolean;
                case TypeCode.String: return String;
                case TypeCode.DateTime: return DateTime;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return Number;
                default: 
                    throw new NotSupportedException("Data type '" + t.Name + "' is not supported by the Google Visualization API.");
            }
        }

        public const string Boolean = "boolean";
        public const string Number = "number";
        public const string String = "string";
        public const string Date = "date";
        public const string DateTime = "datetime";
        public const string Time = "timeofday";
    }

    public class GoogleVisualizationRow
    {
        [JsonProperty("c")] public IList<GoogleVisualizationCell> Cells { get; set; }
        [JsonProperty("p")] public IDictionary<string, string> Attributes { get; set; } 
    }

    public struct GoogleVisualizationCell
    {
        [JsonProperty("v")] public object Value { get; set; }
        [JsonProperty("f")] public string Format { get; set; }
        [JsonProperty("p")] public IDictionary<string, string> Attributes { get; set; } 
    }

    public class GoogleVisualizationResponse
    {
        public GoogleVisualizationResponse()
        {
        }

        public GoogleVisualizationResponse(GoogleVisualizationRequest request)
        {
            Version = request.Version;
            RequestId = request.RequestId;
            Status = GoogleVisualizationResponseStatus.Ok;
        }

        public void Error(string reason, string message, string detail)
        {
            Status = GoogleVisualizationResponseStatus.Error;
            Errors = new [] {
                new GoogleVisualizationResponseWarning {
                    Reason = reason,
                    Message = message,
                    Details = detail,
                },
            };
        }

        public void Warning(string reason, string message, string detail)
        {
            Status = GoogleVisualizationResponseStatus.Warning;
            Warnings = new [] {
                new GoogleVisualizationResponseWarning {
                    Reason = reason,
                    Message = message,
                    Details = detail,
                },
            };
        }

        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("reqId")] public int RequestId { get; set; }

        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("warnings")] public GoogleVisualizationResponseWarning[] Warnings { get; set; }
        [JsonProperty("errors")] public GoogleVisualizationResponseWarning[] Errors { get; set; }

        [JsonProperty("sig")] public string Hash { get; set; }
        [JsonProperty("table")] public GoogleVisualizationTable Table { get; set; }
    }

    public class GoogleVisualizationResponseWarning
    {
        [JsonProperty("reason")] public string Reason { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("detailed_message")] public string Details { get; set; }
    }

    public static class GoogleVisualizationResponeWarningReasons
    {
        public const string DataTruncated = "data_truncated";
        public const string Other = "other";
    }

    public static class GoogleVisualizationResponeErrorReasons
    {
        public const string NotModified = "not_modified";
        public const string NotAuthenticated = "user_not_authenticated";
        public const string UnknownDataSourceId = "unknown_data_source_id";
        public const string AccessDenied = "access_denied";
        public const string UnsupportedQueryOperation = "unsupported_query_operation";
        public const string InvalidQuery = "invalid_query";
        public const string InvalidRequest = "invalid_request";
        public const string InternalError = "internal_error";
        public const string NotSupported = "not_supported";
        public const string IllegalFormattingPatterns = "illegal_formatting_patterns";
        public const string Other = "other";
    }

    public static class GoogleVisualizationResponseStatus
    {
        public const string Ok = "ok";
        public const string Warning = "warning";
        public const string Error = "error";
    }

    public enum GoogleVisualizationResponseFormat
    {
        JSON,
        Html,
        Csv,
        Tsv,
    }

    public class GoogleVisualizationRequest
    {
        public string Query { get; set; }
        public int RequestId { get; set; }
        public string Version { get; set; }
        public string Signature { get; set; }

        public GoogleVisualizationResponseFormat OutputFormat { get; set; }
        public string ResponseHandler { get; set; }
        public string OutputFilename { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public static GoogleVisualizationRequest FromRequest(HttpRequestBase request)
        {
            var r = new GoogleVisualizationRequest();
            r.Query = request.QueryString["tq"];
            
            var tqx = request.QueryString["tqx"];
            if (!String.IsNullOrEmpty(tqx))
            {
                tqx.Split(';').Each(x => {
                    var k = x.Substring(0, x.IndexOf(':'));
                    var v = x.Substring(k.Length+1);
                    switch (k)
                    {
                        case "reqId": r.RequestId = int.Parse(v); break;
                        case "version": r.Version = v; break;
                        case "sig": r.Signature = v; break;
                        case "out":
                            switch (v)
                            {
                                case "json": r.OutputFormat = GoogleVisualizationResponseFormat.JSON; break;
                                case "html": r.OutputFormat = GoogleVisualizationResponseFormat.Html; break;
                                case "csv": r.OutputFormat = GoogleVisualizationResponseFormat.Csv; break;
                                case "tsv-excel": r.OutputFormat = GoogleVisualizationResponseFormat.Tsv; break;
                            }
                            break;
                        case "responseHandler": r.ResponseHandler = v; break;
                        case "outFileName": r.OutputFilename = v; break;
                        default:
                            (r.Parameters ?? (r.Parameters = new Dictionary<string, string>()))
                                .Add(k, v);
                            break;
                    }
                });
            }

            return r;
        }
    }

    public class GoogleVisualizationResult : ActionResult
    {
        public GoogleVisualizationResult()
        {
        }

        public GoogleVisualizationResult(HttpRequestBase request)
        {
            Request = GoogleVisualizationRequest.FromRequest(request);
            Response = new GoogleVisualizationResponse(Request);
        }

        public GoogleVisualizationRequest Request { get; set; }
        public GoogleVisualizationResponse Response { get; set; }

        public void ExecuteResultJson(ControllerContext context)
        {
            var r = context.HttpContext.Response;
            r.ContentType = "application/json";

            // if the request was made with X-DataSource-Auth, use a raw json object,
            // otherwise use a jsonp style response
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;    
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            if (null != context.HttpContext.Request.Headers["X-DataSource-Auth"])
            {
                serializer.Serialize(r.Output, Response);
            }
            else
            {
                r.Write(String.IsNullOrEmpty(Request.ResponseHandler)
                    ? "google.visualization.Query.setResponse"
                    : Request.ResponseHandler);
                r.Write("(");
                serializer.Serialize(r.Output, Response);
                r.Write(")");
            }
        }

        public void ExecuteResultHtml(ControllerContext context)
        {
            throw new NotSupportedException("Html output is not yet supported.");
        }

        public void ExecuteResultCsv(ControllerContext context)
        {
            var r = context.HttpContext.Response;
            r.AddHeader("Content-Disposition", String.Format("attachment; filename={0}_{1:yyyy-MM-dd}.csv", context.RouteData.GetRequiredString("Action"), DateTime.Now));
            r.ContentType = "text/csv";

            r.Write(String.Join(",", Response.Table.Columns.Select(x => x.Label)));
            
            foreach (var row in Response.Table.Rows)
            {
                r.Write('\n');
                r.Write(String.Join(",", row.Cells.Select(x => Convert.ToString(x.Value))));
            }
        }

        public void ExecuteResultTsv(ControllerContext context)
        {
            throw new NotSupportedException("Tsv output is not yet supported.");
        }

        public override void ExecuteResult(ControllerContext context)
        {
            switch (Request.OutputFormat)
            {
                case GoogleVisualizationResponseFormat.JSON:
                    ExecuteResultJson(context);
                    break;
                case GoogleVisualizationResponseFormat.Html:
                    ExecuteResultHtml(context);
                    break;
                case GoogleVisualizationResponseFormat.Csv:
                    ExecuteResultCsv(context);
                    break;
                case GoogleVisualizationResponseFormat.Tsv:
                    ExecuteResultTsv(context);
                    break;

            }
        }
    }

}