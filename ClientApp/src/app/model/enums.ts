export enum ViewMode {
    Add = "Add",
    Edit = "Edit"
  }

  export enum AllFilterTypes {
    number = "number",
    numberFrom = "numberFrom",
    numberTo = "numberTo",
    text = "text",
    list = "list",
    multiSelect = "multiSelect",
    date = "date",
    dateFrom = "dateFrom",
    dateTo = "dateTo",
    filtrowanie = "filtrowanie",
    bit="bit"
  }

  export enum AllAttributeTypes {
    number = "number",
    text = "text",
    list = "list",
    multiSelect = "multiSelect",
    date = "date"
  }

  export enum AttributeTypesForNewProduct {
    number = "number",
    text = "text",
    date = "date",
    bit = "bit"
  }

  export enum ViewTextFilterTypes {
    text = "text",
    list = "list",
    multiSelect = "multiSelect",
    multiSelectOpened = "multiSelectOpened"
}

export enum OrderStatus {
  waitingForPayment = "Nieopłacone",
  preparing = "Przygotowanie paczki",
  sent = "Wysłano",
  returnShipment = "Zwrócono",
  canceled = "Anulowano",
  paymentCompleted = "Opłacono"
}

export enum OrderStatusEng {
  waitingForPayment = "waitingForPayment",
  preparing = "preparing",
  sent = "sent",
  returnShipment = "returnShipment",
  canceled = "canceled",
  paymentCompleted = "paymentCompleted"
}

export enum AccountOperationStatus {
  Add = 10,
  WithdrawWaitingForConfirmation = 20,
  WthdrawPending = 30,
  WithdrawCompleted = 40
}
