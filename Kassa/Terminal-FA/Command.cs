
namespace KitCashProtocol
{
    enum Command
    {
        GET_STATUS = 0x01,
        GET_ZN = 0x02,
        GET_MODEL = 0x04,
        GET_FN = 0x05,
        Input_DATATIME = 0x72,
        GET_DATATIME = 0x73,
        GET_PARAMETERS_OFD = 0x77,

        GET_VERS_CONFIG = 0x0B,
        GET_FISCAL_STORAGE_STATUS = 0x08,
        BEGIN_OPEN_SESSION = 0x21,
        OPEN_SESSION = 0x22,
        BEGIN_CLOSE_SESSION = 0x29,
        CLOSE_SESSION = 0x2A,
        BEGIN_CHECK = 0x23,
        CHECK_POSITION = 0x2B,
        AGENT_DATA = 0x2C,
        PAYMENT_DATA = 0x2D,
        CHECK = 0x24,
        PRINT = 0x61,
        CUT = 0x62,
        
        REGISTRATION_PARAMETERS = 0x0A,
        CANCEL_DOCUMENT = 0x10
    }
}