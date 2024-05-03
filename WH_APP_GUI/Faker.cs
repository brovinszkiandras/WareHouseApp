using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WH_APP_GUI
{
    internal class Faker
    {
        public static void FakeDefaultTables()
        {
            FakeAdmins();
            FakeCeos();
            FakeEmployees();
            FakeProducts();
            FakeWarehouses();
            FakeProductsToWarehouses();
            FakeOrders();
            FakeRevenue();
        }

        public static void FakeAdmins()
        {
            //Admin
            //password: Sp03Gu9c
            SQL.SqlCommand("INSERT INTO `staff`(`name`, `email`, `password`, `role_id`) " +
                    "VALUES ('Alex Johnson', 'alex.johnson@example.com', '50099b41c4bec39fb9f856a410060a6350522eb02a2e69aa9237b1c04a37584c', 1)");

            //password: 6857rw9V
            SQL.SqlCommand("INSERT INTO `staff`(`name`, `email`, `password`, `role_id`) " +
                    "VALUES ('Samira Rai', 'samira.rai@example.com', '34cc25c535fbdd78b56d52973ce1f749698c155bc2ad66e20d1785693bcabb8a', 1)");

            //password: P37S15e8R2O
            SQL.SqlCommand("INSERT INTO `staff`(`name`, `email`, `password`, `role_id`) " +
                    "VALUES ('Lukas Bauer', 'lukas.bauer@example.com', '2e22183d00613ab68169ddb6c969c0ccd570d773feba92de26de652538f67312', 1)");
        }
        public static void FakeCeos()
        {
            //Ceo
            //password: Xf51R9qM3y
            SQL.SqlCommand("INSERT INTO `staff`(`name`, `email`, `password`, `role_id`) " +
                    "VALUES ('Alex Mercer', 'alex.mercer@example.com', '7255c31aa6b994fc22322499ee68bff7d83403f0e93b1e3e4e30522548c23a21', 2)");

            //password: DT9r7Rg3
            SQL.SqlCommand("INSERT INTO `staff`(`name`, `email`, `password`, `role_id`) " +
                    "VALUES ('Jordan Lin', 'jordan.lin@example.com', '4af3e5c40dcb31186e82a6c066758678eb53f9232ac857df2696b5944f81f733', 2)");
        }
        public static void FakeEmployees()
        {
            //Bp267oo
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('John Smith', 'john.smith@example.com','ad66270b31740d6c7d08564999788584c24c074dde8321aa36fe46921923db2c', 3, 1);");
            //0RUyfp8
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('Emily Johnson', 'emily.johnson@example.com', '6f16ca3ab809ec2731a6a38aa80cb974c9f1b2eb0d557a69a0c804004bd2dea8', 3, 1);");
            //ixJ90h0fvma
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('Michael Brown', 'michael.brown@example.com', 'ce41895b62241143f955db89ba2fe594cb524b8312603139964fd8642c039e30', 4, 1);");

            //Fe577WFbJJ4g
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('Linda Davis', 'linda.davis@example.com', 'b9ba00351f39373e4c34cde7df74576685de9bac63d751b845429de291bad4bf', 3, 2);");
            //gXH0nnH
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('Robert Wilson', 'robert.wilson@example.com', '72e049e9017a9e4840004d4c54dd251344ec4ecd4ea4828ec631c7d0a1950779', 3, 2);");
            //DGwD5UB54
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('Patricia Miller', 'patricia.miller@example.com', 'c713025b7e01211b50eb83584f6b23a994d8ee856051bfb09a8569966d5a50fa', 4, 2);");

            //G4L5zgNKd80
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('James Taylor', 'james.taylor@example.com', '23c872f432cd1fab57860dde14ab36ff78eff59ab689406030a8984e1e58e922', 5, 3);");
            //5416GMoG6MDY
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('Barbara Moore', 'barbara.moore@example.com', 'c91fc7fd87c0569d9978e58a6692190001ad422a118e421b55fb2eb0e5d27b31', 6, 3);");
            //0jFk60
            SQL.SqlCommand("INSERT INTO `employees`(`name`, `email`, `password`, `role_id`, `warehouse_id`) VALUES ('William Anderson', 'william.anderson@example.com', 'a5dc140ac79ed48d4bdb3d0cd313cebd24afa122056d8cd5965330a50db4c13e', 3, 3);");
        }
        public static void FakeWarehouses()
        {
            //WH1
            SQL.SqlCommand("INSERT INTO `warehouses`(`name`, `length`, `width`, `height`, `volume`, `city_id`) " +
                "VALUES ('central_storage_hub', 200, 150, 15, 450000, 1)");

            Controller.CreateWarehouse("central_storage_hub");

            SQL.SqlCommand("INSERT INTO `sector` (`name`, `length`, `width`, `area`, `area_in_use`, `warehouse_id`) " +
                "VALUES('Sector One CSH', 10, 10, 100, 1.75, 1)," +
                "('Sector Two CSH', 10, 10, 100, 5.5, 1)," +
                "('Sector Three CSH', 10, 10, 100, 4.75, 1);");

            SQL.SqlCommand("INSERT INTO `shelf` (`name`, `number_of_levels`, `length`, `actual_length`, `width`, `sector_id`, `startXindex`, `startYindex`, `orientation`) " +
                "VALUES('Shelf One CSH', 1, 4, 4, 1, 1, 2, 3, 'Vertical')," +
                "('Shelf Two CSH', 1, 8, 8, 1, 2, 1, 3, 'Horizontal')," +
                "('Shelf Three CSH', 1, 8, 8, 1, 2, 1, 5, 'Horizontal')," +
                "('Shelf Four CSH', 1, 6, 6, 1, 3, 0, 3, 'Horizontal')," +
                "('Shelf Five CSH', 1, 7, 7, 1, 3, 3, 5, 'Horizontal');");

            //WH2
            SQL.SqlCommand("INSERT INTO `warehouses`(`name`, `length`, `width`, `height`, `volume`, `city_id`) " +
                "VALUES ('eastside_distribution_center', 250, 100, 10, 250000, 8)");

            Controller.CreateWarehouse("eastside_distribution_center");

            SQL.SqlCommand("INSERT INTO `sector` (`name`, `length`, `width`, `area`, `area_in_use`, `warehouse_id`) " +
                "VALUES('Sector One EDC', 10, 10, 100, 1.75, 2)," +
                "('Sector Two EDC', 10, 10, 100, 5.5, 2)," +
                "('Sector Three EDC', 10, 10, 100, 4.75, 2);");

            SQL.SqlCommand("INSERT INTO `shelf` (`name`, `number_of_levels`, `length`, `actual_length`, `width`, `sector_id`, `startXindex`, `startYindex`, `orientation`) " +
                "VALUES('Shelf One EDC', 1, 4, 4, 1, 4, 2, 3, 'Vertical')," +
                "('Shelf Two EDC', 1, 8, 8, 1, 5, 1, 3, 'Horizontal')," +
                "('Shelf Three EDC', 1, 8, 8, 1, 5, 1, 5, 'Horizontal')," +
                "('Shelf Four EDC', 1, 6, 6, 1, 6, 0, 3, 'Horizontal')," +
                "('Shelf Five EDC', 1, 7, 7, 1, 6, 3, 5, 'Horizontal');");

            //WH3
            SQL.SqlCommand("INSERT INTO `warehouses`(`name`, `length`, `width`, `height`, `volume`, `city_id`) " +
                "VALUES ('westgate_logistics_park', 300, 200, 20, 1200000, 13)");

            Controller.CreateWarehouse("westgate_logistics_park");

            SQL.SqlCommand("INSERT INTO `sector` (`name`, `length`, `width`, `area`, `area_in_use`, `warehouse_id`) " +
                "VALUES('Sector One WLP', 10, 10, 100, 1.75, 3)," +
                "('Sector Two WLP', 10, 10, 100, 5.5, 3)," +
                "('Sector Three WLP', 10, 10, 100, 4.75, 3);");

            SQL.SqlCommand("INSERT INTO `shelf` (`name`, `number_of_levels`, `length`, `actual_length`, `width`, `sector_id`, `startXindex`, `startYindex`, `orientation`) " +
                "VALUES('Shelf One WLP', 1, 4, 4, 1, 7, 2, 3, 'Vertical')," +
                "('Shelf Two WLP', 1, 8, 8, 1, 8, 1, 3, 'Horizontal')," +
                "('Shelf Three WLP', 1, 8, 8, 1, 8, 1, 5, 'Horizontal')," +
                "('Shelf Four WLP', 1, 6, 6, 1, 9, 0, 3, 'Horizontal')," +
                "('Shelf Five WLP', 1, 7, 7, 1, 9, 3, 5, 'Horizontal');");
        }
        public static void FakeProducts()
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'"))
            {
                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`, `weight`, `volume`, `width`, `heigth`, `length`) " +
                    "VALUES ('Quantum Processor', 150000, 250000, 'A next-generation processor with quantum computing capabilities for high-speed data processing.', 0.5, 0.00045, 0.045, 0.005, 0.2)");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`, `weight`, `volume`, `width`, `heigth`, `length`) " +
                    "VALUES ('SolarFlex Panel', 75000, 120000, 'Flexible and durable solar panel with high efficiency for renewable energy solutions.', 2, 0.06, 0.8, 0.02, 1.5)");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`, `weight`, `volume`, `width`, `heigth`, `length`) " +
                    "VALUES ('AeroDrone X11', 300000, 450000, 'Advanced drone equipped with AI for aerial photography and surveillance.', 1.2, 0.028, 0.5, 0.2, 0.35)");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`, `weight`, `volume`, `width`, `heigth`, `length`) " +
                    "VALUES ('HydroPurify Filter', 20000, 35000, 'Water purification system using reverse osmosis for clean drinking water.', 3, 0.018, 0.3, 0.4, 0.15)");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`, `weight`, `volume`, `width`, `heigth`, `length`) " +
                    "VALUES ('SmartEco Bulb', 1500, 2500, 'Energy-saving LED bulb with smart home integration for efficient lighting.', 0.1, 0.0004, 0.05, 0.1, 0.05)");
            }
            else
            {
                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`) " +
                    "VALUES ('Quantum Processor', 150000, 250000, 'A next-generation processor with quantum computing capabilities for high-speed data processing.')");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`) " +
                    "VALUES ('SolarFlex Panel', 75000, 120000, 'Flexible and durable solar panel with high efficiency for renewable energy solutions.')");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`) " +
                    "VALUES ('AeroDrone X11', 300000, 450000, 'Advanced drone equipped with AI for aerial photography and surveillance.')");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`) " +
                    "VALUES ('HydroPurify Filter', 20000, 35000, 'Water purification system using reverse osmosis for clean drinking water.')");

                SQL.SqlCommand("INSERT INTO `products`(`name`, `buying_price`, `selling_price`, `description`) " +
                    "VALUES ('SmartEco Bulb', 1500, 2500, 'Energy-saving LED bulb with smart home integration for efficient lighting.')");
            }
        }
        public static void FakeProductsToWarehouses()
        {
            //WH1
            SQL.SqlCommand("INSERT INTO `central_storage_hub` (`product_id`, `qty`, `shelf_id`, `width`, `height`, `length`, `on_shelf_level`, `is_in_box`) " +
                "VALUES(1, 50, 1, 0.045, 0.005, 0.2, 1, 0)," +
                "(2, 20, 2, 0.8, 0.02, 1.5, 1, 0)," +
                "(5, 40, 5, 0.05, 0.1, 0.05, 1, 0);");

            //WH2
            SQL.SqlCommand("INSERT INTO `eastside_distribution_center` (`product_id`, `qty`, `shelf_id`, `width`, `height`, `length`, `on_shelf_level`, `is_in_box`) " +
                "VALUES (4, 70, 8, 0.3, 0.4, 0.15, 1, 0)," +
                "(3, 20, 6, 0.5, 0.2, 0.35, 1, 0)," +
                "(4, 30, 9, 0.3, 0.4, 0.15, 1, 0)," +
                "(2, 10, 6, 0.8, 0.02, 1.5, 1, 0);");

            //WH3
            SQL.SqlCommand("INSERT INTO `westgate_logistics_park` (`product_id`, `qty`, `shelf_id`, `width`, `height`, `length`, `on_shelf_level`, `is_in_box`) " +
                "VALUES (4, 40, 14, 0.3, 0.4, 0.15, 1, 0)," +
                "(5, 60, 13, 0.05, 0.1, 0.05, 1, 0)," +
                "(2, 10, 11, 0.8, 0.02, 1.5, 1, 0);");

        }
        public static void FakeOrders()
        {
            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES(1, 2, 'Processing', 'John Doe', '123 Main St', 'bank card', 45);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES(3, 1, 'Shipped', 'Jane Smith', '456 Elm St', 'with cash on delivery', 118);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES(5, 4, 'Delivered', 'Alex Johnson', '789 Oak St', 'bank card', 217);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (1, 3, 'Registered', 'Freddy Krueger', '1428 Elm Street', 'bank card', 13);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (2, 5, 'Registered', 'Norman Bates', 'Psycho Path', 'with cash on delivery', 27);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (3, 2, 'Registered', 'Hannibal Lecter', 'Chianti Quarters', 'bank card', 86);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (4, 1, 'Registered', 'Jason Voorhees', 'Crystal Lake Ave', 'with cash on delivery', 150);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (5, 4, 'Registered', 'Michael Myers', 'Haddonfield Lane', 'bank card', 199);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (1, 6, 'Registered', 'Leatherface', 'Texas Trail', 'with cash on delivery', 45);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (2, 3, 'Registered', 'Jack Torrance', 'Overlook Hotel', 'bank card', 217);");

            SQL.SqlCommand("INSERT INTO `orders`(`product_id`, `qty`, `status`, `user_name`, `address`, `payment_method`, `city_id`) " +
                "VALUES (3, 7, 'Registered', 'Pennywise', 'Derry Circus', 'with cash on delivery', 89);");

        }
        public static void FakeCars()
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fleet'"))
            {
                if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'") && SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fuel'"))
                {
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('ABC-123', 'Toyota', 1, 5000, '2024-04-30', '2024-05-10', 1, 350, 500, 6, 50);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('DEF-456', 'Honda', 0, 15000, '2024-05-02', '2024-05-12', 2, 300, 450, 5, 55);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('GHI-789', 'Chevrolet', 1, 20000, '2024-05-05', '2024-05-15', 3, 400, 600, 7, 60);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('JKL-012', 'Ford', 1, 30000, '2024-05-08', '2024-05-18', 1, 500, 700, 8, 65);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('MNO-345', 'Tesla', 0, 10000, '2024-05-11', '2024-05-21', 2, 250, 300, 4, 70);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('PQR-678', 'BMW', 1, 8000, '2024-05-14', '2024-05-24', 3, 450, 550, 9, 75);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('STU-901', 'Audi', 1, 6000, '2024-05-17', '2024-05-27', 1, 375, 525, 10, 80);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('VWX-234', 'Mercedes', 1, 4000, '2024-05-20', '2024-05-30', 2, 325, 475, 11, 85);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('YZA-567', 'Nissan', 0, 12000, '2024-05-23', '2024-06-02', 3, 410, 560, 12, 90);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`, `consumption`, `gas_tank_size`) VALUES ('BCD-890', 'Kia', 1, 7000, '2024-05-26', '2024-06-05', 1, 390, 540, 13, 95);");
                }
                else if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'") && !SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fuel'"))
                {
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('ABC-123', 'Toyota', 1, 5000, '2024-04-30', '2024-05-10', 1, 350, 500);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('DEF-456', 'Honda', 0, 15000, '2024-05-02', '2024-05-12', 2, 300, 450);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('GHI-789', 'Chevrolet', 1, 20000, '2024-05-05', '2024-05-15', 3, 400, 600);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('JKL-012', 'Ford', 1, 30000, '2024-05-08', '2024-05-18', 1, 500, 700);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('MNO-345', 'Tesla', 0, 10000, '2024-05-11', '2024-05-21', 2, 250, 300);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('PQR-678', 'BMW', 1, 8000, '2024-05-14', '2024-05-24', 3, 450, 550);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('STU-901', 'Audi', 1, 6000, '2024-05-17', '2024-05-27', 1, 375, 525);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('VWX-234', 'Mercedes', 1, 4000, '2024-05-20', '2024-05-30', 2, 325, 475);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('YZA-567', 'Nissan', 0, 12000, '2024-05-23', '2024-06-02', 3, 410, 560);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `storage`, `carrying_capacity`) VALUES ('BCD-890', 'Kia', 1, 7000, '2024-05-26', '2024-06-05', 1, 390, 540);");

                }
                else if (!SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'") && SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fuel'"))
                {
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('ABC-123', 'Toyota', 1, 5000, '2024-04-30', '2024-05-10', 1, 6, 50);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('DEF-456', 'Honda', 0, 15000, '2024-05-02', '2024-05-12', 2, 5, 55);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('GHI-789', 'Chevrolet', 1, 20000, '2024-05-05', '2024-05-15', 3, 7, 60);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('JKL-012', 'Ford', 1, 30000, '2024-05-08', '2024-05-18', 1, 8, 65);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('MNO-345', 'Tesla', 0, 10000, '2024-05-11', '2024-05-21', 2, 4, 70);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('PQR-678', 'BMW', 1, 8000, '2024-05-14', '2024-05-24', 3, 9, 75);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('STU-901', 'Audi', 1, 6000, '2024-05-17', '2024-05-27', 1, 10, 80);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('VWX-234', 'Mercedes', 1, 4000, '2024-05-20', '2024-05-30', 2, 11, 85);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('YZA-567', 'Nissan', 0, 12000, '2024-05-23', '2024-06-02', 3, 12, 90);");
                    SQL.SqlCommand("INSERT INTO `cars` (`plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`, `consumption`, `gas_tank_size`) VALUES ('BCD-890', 'Kia', 1, 7000, '2024-05-26', '2024-06-05', 1, 13, 95);");

                }
                else if (!SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Storage'") && !SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Fuel'"))
                {
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (1, 'ABC-123', 'Toyota', 1, 5000, '2024-04-30', '2024-05-10', 1);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (2, 'DEF-456', 'Honda', 0, 15000, '2024-05-02', '2024-05-12', 2);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (3, 'GHI-789', 'Chevrolet', 1, 20000, '2024-05-05', '2024-05-15', 3);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (4, 'JKL-012', 'Ford', 1, 30000, '2024-05-08', '2024-05-18', 1);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (5, 'MNO-345', 'Tesla', 0, 10000, '2024-05-11', '2024-05-21', 2);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (6, 'PQR-678', 'BMW', 1, 8000, '2024-05-14', '2024-05-24', 3);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (7, 'STU-901', 'Audi', 1, 6000, '2024-05-17', '2024-05-27', 1);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (8, 'VWX-234', 'Mercedes', 1, 4000, '2024-05-20', '2024-05-30', 2);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (9, 'YZA-567', 'Nissan', 0, 12000, '2024-05-23', '2024-06-02', 3);");
                    SQL.SqlCommand("INSERT INTO `cars` (`id`, `plate_number`, `type`, `ready`, `km`, `last_service`, `last_exam`, `warehouse_id`) VALUES (10, 'BCD-890', 'Kia', 1, 7000, '2024-05-26', '2024-06-05', 1);");
                }
            }
        }
        public static void FakeRevenue()
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Revenue'"))
            {
                //WH 1
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (1, '2024-05-02', 500000, 750000);");
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (1, '2024-05-05', 450000, 700000);");
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (1, '2024-05-08', 550000, 800000);");

                //WH 2
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (2, '2024-05-03', 600000, 850000);");
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (2, '2024-05-06', 650000, 900000);");
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (2, '2024-05-09', 700000, 950000);");

                //WH 3
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (3, '2024-05-04', 700000, 950000);");
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (3, '2024-05-07', 750000, 1000000);");
                SQL.SqlCommand("INSERT INTO `revenue_a_day`(`warehouse_id`, `date`, `total_expenditure`, `total_income`) VALUES (3, '2024-05-10', 800000, 1050000);");
            }
        }
        public static void FakeDocks()
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Dock'"))
            {
                //WH 1
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock A1', 1, 1);");
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock A2', 0, 1);");
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock A3', 1, 1);");

                //WH 2
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock B1', 1, 2);");
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock B2', 0, 2);");
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock B3', 1, 2);");

                //WH 3
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock C1', 1, 3);");
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock C2', 0, 3);");
                SQL.SqlCommand("INSERT INTO `dock` (`name`, `free`, `warehouse_id`) VALUES ('Dock C3', 1, 3);");
            }
        }
        public static void FakeForklifts()
        {
            if (SQL.BoolQuery("SELECT in_use FROM feature WHERE name = 'Forklift'"))
            {
                //WH 1
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (1, 'LiftMaster 3000', 'Free', 1200);");
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (1, 'HighStacker 500', 'On duty', 800);");
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (1, 'PalletPro X', 'Free', 1500);");

                //WH 2
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (2, 'CargoKing 200', 'On duty', 1000);");
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (2, 'StackWizard 750', 'On duty', 700);");
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (2, 'LoadLifter 900', 'On duty', 1300);");

                //WH 3
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (3, 'MegaMover 400', 'Under Maintenance', 1100);");
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (3, 'PalletChief 1000', 'Under Maintenance', 750);");
                SQL.SqlCommand("INSERT INTO `forklift`(`warehouse_id`, `type`, `status`, `operating_hours`) VALUES (3, 'LiftBoss 800', 'Faulty', 1400);");
            }
        }

        public static void FakeEveryting()
        {
            FakeAdmins();
            FakeCeos();
            FakeWarehouses();
            FakeEmployees();
            FakeProducts();
            FakeProductsToWarehouses();
            FakeOrders();
            FakeCars();
            FakeRevenue();
            FakeDocks();
            FakeForklifts();
        }
    }
}
