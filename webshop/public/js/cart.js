
    function changeQuantity(id){
    
    let quantity = document.getElementById(id).value;
    

    fetch('http://localhost:3000/quantityChange', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ number: quantity, id: id})
        })
    }

   
    
