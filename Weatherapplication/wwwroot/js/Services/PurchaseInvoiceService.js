app.service("PurchaseInvoiceService", function ($http) {

    this.GetCustomerList = function () {
        return $http.get("/api/PurchaseInvoiceapi/GetCustomerList");
    };

    this.GetItemist = function () {
        return $http.get("/api/PurchaseInvoiceapi/GetItemist");
    };

    this.Save = function (customer) {
        return $http.post("/api/PurchaseInvoiceapi/Save", customer);
    };

    this.GetAll = function () {
        return $http.get("/api/PurchaseInvoiceapi/GetAll");
    };

    this.GetById = function (id) {
        return $http.get("/api/PurchaseInvoiceapi/GetById/" + id);
    };

    this.Update = function (customer) {
        return $http.put("/api/PurchaseInvoiceapi/Update", customer);
    };

    this.Delete = function (id) {
        return $http.delete("/api/PurchaseInvoiceapi/Delete/" + id);
    };
    this.GetNextPurchaseInvoiceNo = function () {
        return $http.get('/api/PurchaseInvoiceapi/GetNextPurchaseInvoiceNo');
    };
    this.ConverttoPurchaseInvoice = function (id) {
        return $http.get("/api/PurchaseInvoiceapi/ConverttoPurchaseInvoice/" + id);
    };
});