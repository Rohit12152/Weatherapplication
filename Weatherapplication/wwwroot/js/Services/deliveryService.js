app.service("deliveryService", function ($http) {

    this.GetCustomerList = function () {
        return $http.get("/api/DeliveryOrderapi/GetCustomerList");
    };

    this.GetItemist = function () {
        return $http.get("/api/DeliveryOrderapi/GetItemist");
    };
    this.GetItemcategory = function () {
        return $http.get("/api/DeliveryOrderapi/GetItemcategory");
    };
    this.Save = function (customer) {
        return $http.post("/api/DeliveryOrderapi/Save", customer);
    };
    this.GetItemListByCategory = function (categoryId) {
        return $http.get(apiUrl + "/GetItemListByCategory?categoryId=" + categoryId);
    };
    this.GetAll = function () {
        return $http.get("/api/DeliveryOrderapi/GetAll");
    };

    this.GetById = function (id) {
        return $http.get("/api/DeliveryOrderapi/GetById/" + id);
    };

    this.Update = function (customer) {
        return $http.put("/api/DeliveryOrderapi/Update", customer);
    };

    this.Delete = function (id) {
        return $http.delete("/api/DeliveryOrderapi/Delete/" + id);
    };
    this.GetNextDONo = function () {
        return $http.get('/api/DeliveryOrderapi/GetNextDONo');
    };
    this.ConverttoDO = function (id) {
        return $http.get("/api/DeliveryOrderapi/ConverttoDO/" + id);
    };
});