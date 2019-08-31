var cart = [];
var allCount = 0;
var allPrice = 0;

$(document).ready(function () {
    $('.add').click(function () {
        var orderDetail = {
            dishId: $(this).data('id'),
            price: $(this).data('price'),
            quantity: 1,
        }
        allCount += 1;
        allPrice += orderDetail.price;
        $("#allCount").text(allCount);
        $("#allPrice").text(allPrice);
        var finded_dish = cart.find((item) => item.dishId == $(this).data('id'));
        if (finded_dish != undefined) {
            orderDetail = finded_dish;
            orderDetail.quantity++;
            cart = cart.filter((item) => item.dishId !== $(this).data('id'));
            cart.push(orderDetail);
            $(this).siblings('.count').text(orderDetail.quantity);
        }
        else {
            cart.push(orderDetail);
            $(this).siblings('.count').text(orderDetail.quantity);
        }

    }
    );
    $('.remove').click(function () {
        var findedOrderDetail = cart.find((item) => item.dishId == $(this).data('id'));
        if (findedOrderDetail != null) {
            allCount -= 1;
            allPrice -= findedOrderDetail.price;
            $("#allCount").text(allCount);
            $("#allPrice").text(allPrice);
            findedOrderDetail.quantity--;
            if (findedOrderDetail.quantity <= 0) {
                cart = cart.filter((item) => item.dishId !== $(this).data('id'));
                if (findedOrderDetail.quantity == 0)
                    $(this).siblings('.count').text(0);
                else
                    $(this).siblings('.count').text(findedOrderDetail.quantyty);
            }
            else {
                cart = cart.filter((item) => item.dishId !== $(this).data('id'));
                cart.push(findedOrderDetail);
                $(this).siblings('.count').text(findedOrderDetail.quantity);
            }
        }
    }
    );
    $('#createOrder').click(function () {
        if (cart.length > 0) {
            if ($("#table").val() != null) {
                var tableId = $('#table').val();
                var shiftId = $('#shift').val();
                var order = {
                    tableId: tableId,
                    shiftId: shiftId,
                    finalPrice: allPrice
                }
                var cartCopy = cart.copyWithin();
                cartCopy.forEach(function (item) {
                    delete item.price
                }
                );
                var json = {
                    "OrderDetails": cartCopy,
                    "Order": order
                }
                json = JSON.stringify(json);
                console.log(json);
                $.ajax({
                    type: 'POST',
                    url: '/Waiter/NewOrder/',
                    contentType: "application/json; charset=utf-8",
                    data: json,
                    success: function (data) {
                        window.location.href = '/Waiter/Index/' + data;
                    },
                    error: function () {
                        alert('Не удалось сформировать заказ')
                    }
                })
            }
            else {
                alert('Не выбран столик');
                return;
            }
        }
        else {
            alert('Не выбраны блюда');
            return;
        }
    })
})