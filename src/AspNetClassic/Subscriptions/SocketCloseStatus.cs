namespace HotChocolate.AspNetClassic.Subscriptions;

public enum SocketCloseStatus
{
    None,
    NormalClosure,
    EndpointUnavailable,
    ProtocolError,
    InvalidMessageType,
    InvalidPayloadData,
    PolicyViolation,
    MessageTooBig,
    MandatoryExtension,
    InternalServerError
}
