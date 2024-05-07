const express = require('express');

const bodyParser = require('body-parser');

const app = express();

const nocache = require('nocache');

const session = require('express-session');

app.use(session({
    secret: 'your_secret_key_here',
    resave: false,
    saveUninitialized: true
}));

const mysql = require('mysql');

const database = mysql.createPool({
    host: '127.0.0.1',
    user: 'root',
    password: '',
    database: 'wh_app_database'
});

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

app.set('view engine', 'pug');

app.use(express.static("node_modules/bootstrap/dist/js/"));
app.use(express.static("node_modules/bootstrap/dist/css/"));
app.use(express.static("public/js/"));
app.use(express.static("public/css/"));
app.use(express.static("public/images/"));


app.use(nocache());


app.get('/', (req, res) => {
    
    database.query('SELECT * FROM products', (error, results, fields) => {
        if (error) {
            console.error('Error executing query:', error);
            return;
        }
      
       res.render('products/index', {"products": results});
    });
})

class Cart_Item {
    constructor() {
      this.id = null;
      this.productName = null;
      this.quantity = 1;
      this.price = null;
    }
  }

var cart = [];

app.post('/addToCart/:product', function (req, res) {
    const product = JSON.parse( req.params.product);
    if(!req.session.cart){

        req.session.cart = [];
    }

    let alreadyHasItems = false;
    for (let i = 0; i < req.session.cart.length; i++) {
      let item = req.session.cart[i];
  
  
      if (item.id === product["id"]) {
        item.quantity++;
        alreadyHasItems = true;
  
        break;
      }
  
    }
    if (alreadyHasItems == false) {
      var cartItem = new Cart_Item();
      cartItem.id = product["id"];
      cartItem.productName = product["name"];
      cartItem.price = product["selling_price"];
  
      req.session.cart.push(cartItem);
    }
    res.redirect('/');
})
  
app.get('/products/cart', (req, res) => {
  // Recalculate sumPrice based on updated quantities
  
  req.session.sumPrice = 0;

  if (req.session.cart && req.session.cart.length !== 0) {
      for (let i = 0; i < req.session.cart.length; i++) {
        
          let item = req.session.cart[i];
          req.session.sumPrice += item.price * item.quantity;
      }
  }

  //console.log("sumPrice: " + req.session.sumPrice);
  res.render('products/cart', { "cart": req.session.cart, "sumPrice": req.session.sumPrice });
});


app.post('/quantityChange', function (req, res) {
  console.log('Received number:', req.body);
  let receivedNumber = req.body.number;
  let id = req.body.id;

  for (let i = 0; i < req.session.cart.length; i++) {
      let item = req.session.cart[i];
      if (item.id == id) {
          item.quantity = receivedNumber;
          break;
      }
  }
  // Redirect to /products/cart after updating quantities
  res.redirect('/products/cart');
});

app.post('/cart/delete:id', function (req, res) {
  let id = req.params.id;
  for (let index = 0; index < req.session.cart.length; index++) {
    const element = req.session.cart[index];
    console.log(element.id);
    
    if(element.id == id) {
      console.log("found it");
      req.session.cart.splice(index, 1);
      break;
    }
  }
  console.log("Item number " + id + "Removed from cart")

  res.redirect('/products/cart');
})

app.get('/payment', (req, res) => {

  database.query('SELECT * FROM cities', (error, results, fields) => {
    if (error) {
        console.error('Error executing query:', error);
        return;
    }

    // Save the result in a variable
  let cities = results;

    // Do something with the query result
    
    console.log("ready to spend your money");
  res.render('products/payment', {"cart": req.session.cart, "sumPrice": req.session.sumPrice, "cities": cities});
});

  
})

app.post('/sendOrder', function (req, res) {


  for (let i = 0; i < req.session.cart.length; i++) {
    console.log(i);
    const element = req.session.cart[i];
    let order = {product_id: element.id, 
      qty: element.quantity, 
      user_name: req.body.user_name,
      address: req.body.address,
      payment_method: req.body.payment_method,
      city_id: req.body.city_id}

      database.query("INSERT INTO orders SET ?", order, (err, result) => {
        if (err) throw err;
        console.log('Inserted row with ID:', result.insertId);
      });
  }
  req.session.cart.length = 0;
  

  res.redirect("/");
})

app.listen(3000);