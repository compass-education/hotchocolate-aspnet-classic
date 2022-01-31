using System;
using HotChocolate.Language;

namespace HotChocolate.AspNetClassic.Server
{

    /// <summary>
    /// An internal helper class that centralizes the server exceptions.
    /// </summary>
    internal static class ThrowHelper
    {
        public static GraphQLRequestException DefaultHttpRequestParser_QueryAndIdMissing() =>
            new GraphQLRequestException(ErrorBuilder.New()
                .SetMessage("Either the parameter query or the parameter id has to be set.")
                .SetCode(ErrorCodes.Server.QueryAndIdMissing)
                .Build());

        public static GraphQLRequestException DefaultHttpRequestParser_SyntaxError(
            SyntaxException ex) =>
            new GraphQLRequestException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .AddLocation(ex.Line, ex.Column)
                .SetCode(ErrorCodes.Server.SyntaxError)
                .Build());

        public static GraphQLRequestException DefaultHttpRequestParser_UnexpectedError(
            Exception ex) =>
            new GraphQLRequestException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetException(ex)
                .SetCode(ErrorCodes.Server.UnexpectedRequestParserError)
                .Build());

        public static GraphQLRequestException DefaultHttpRequestParser_RequestIsEmpty() =>
            new GraphQLRequestException(ErrorBuilder.New()
                .SetMessage("The GraphQL request is empty")
                .SetCode(ErrorCodes.Server.RequestInvalid)
                .Build());

        public static GraphQLRequestException DefaultHttpRequestParser_MaxRequestSizeExceeded() =>
            new GraphQLRequestException(ErrorBuilder.New()
                .SetMessage("Max GraphQL request size reached.")
                .SetCode(ErrorCodes.Server.MaxRequestSize)
                .Build());

        public static NotSupportedException DataStartMessageHandler_RequestTypeNotSupported() =>
            new NotSupportedException("ThrowHelper_DataStartMessageHandler_RequestTypeNotSupported");

        public static GraphQLException HttpMultipartMiddleware_Invalid_Form(
            Exception ex) =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("The multipart form could not be read.")
                    .SetException(ex)
                    .SetCode(ErrorCodes.Server.MultiPartInvalidForm)
                    .SetExtension("underlyingError", ex.Message)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_No_Operations_Specified() =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("No 'operations' specified.")
                    .SetCode(ErrorCodes.Server.MultiPartNoOperationsSpecified)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_Fields_Misordered() =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("Misordered multipart fields; 'map' should follow 'operations'.")
                    .SetCode(ErrorCodes.Server.MultiPartFieldsMisordered)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_NoObjectPath(string filename) =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("No object paths specified for key '{0}' in 'map'.", filename)
                    .SetCode(ErrorCodes.Server.MultiPartNoObjectPath)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_FileMissing(string filename) =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("File of key '{0}' is missing.", filename)
                    .SetCode(ErrorCodes.Server.MultiPartFileMissing)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_VariableNotFound(string path) =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("The variable path '{0}' is invalid.", path)
                    .SetCode(ErrorCodes.Server.MultiPartVariableNotFound)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_VariableStructureInvalid() =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("The variable structure is invalid.")
                    .SetCode(ErrorCodes.Server.MultiPartVariableStructureInvalid)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_InvalidPath(string path) =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("Invalid variable path `{0}` in `map`.", path)
                    .SetCode(ErrorCodes.Server.MultiPartInvalidPath)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_PathMustStartWithVariable() =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("The variable path must start with `variables`.")
                    .SetCode(ErrorCodes.Server.MultiPartPathMustStartWithVariable)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_InvalidMapJson() =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("Invalid JSON in the `map` multipart field; Expected type of Dictionary&lt;string, string[]&gt;.")
                    .SetCode(ErrorCodes.Server.MultiPartInvalidMapJson)
                    .Build());

        public static GraphQLException HttpMultipartMiddleware_MapNotSpecified() =>
            new GraphQLRequestException(
                ErrorBuilder.New()
                    .SetMessage("No `map` specified.")
                    .SetCode(ErrorCodes.Server.MultiPartMapNotSpecified)
                    .Build());
    }
}
