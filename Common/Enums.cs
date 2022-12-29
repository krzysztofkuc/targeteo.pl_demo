using System;
using System.ComponentModel;
using static targeteo.pl.Common.Enums;

namespace targeteo.pl.Common
{
    [Serializable]
    public static class Enums
    {
        public enum ViewMode {add, edit};
        //leave it in the same order
        public enum PostType {img, gif, mov, link};

        public enum UIPostType { Humour, MainMeme, Suchar, Article };

        public enum PictureType { img, gif };

        public enum AllAttributeTypes { number, numberFrom, numberTo, text, list, multiSelect, date, dateFrom, dateTo, filtrowanie, from, to, orderBy, bit };

        public enum ViewFilterTypes { text, list, multiSelect, multiSelectOpened, bit };

        public enum SupplyMethodType { defined = 1, custom = 0};

        public enum OrderStatus 
        {
            [Description("Nieopłacone")] 
            waitingForPayment,

            [Description("Przygotowanie paczki")]
            preparing,

            [Description("Wysłano")]
            sent,

            [Description("Zwrócono")]
            returnShipment,

            [Description("Anulowano")]
            canceled,

            [Description("Opłacono")]
            paymentCompleted,

            [Description("Potwierdzono odbiór")]
            receiptConfirmed,
        };

        public enum AccountOperationStatus
        {
            [Description("Wpłacono")]
            Add = 10,

            [Description("Wypłata czeka na potwierdzenie")]
            WithdrawWaitingForConfirmation = 20,

            [Description("Wypłata w trakcie realizacji")]
            WthdrawPending = 30,

            [Description("Wypłata zrealizowana")]
            WithdrawCompleted = 40
        };
    }

    public static class PosTypeExtensions
    {
        public static string ToFriendlyString(this PostType me)
        {
            switch (me)
            {
                case PostType.img:
                    return "img";
                case PostType.gif:
                    return "gif";
                case PostType.mov:
                    return "mov";
                case PostType.link:
                    return "link";
                default:
                    return "damn";
            }
        }
    }
}