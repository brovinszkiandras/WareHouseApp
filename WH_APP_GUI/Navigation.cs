using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WH_APP_GUI.Warehouse;

namespace WH_APP_GUI
{
    internal class Navigation
    {
        public static Frame content2 = new Frame();
        public static Page PreviousPage = null;
        public static object ReturnParam = null;
        public static bool SkipParam = false;
        public static void RemoveParent()
        {
            if (content2.Parent != null)
            {
                Grid parentGrid = content2.Parent as Grid;
                parentGrid.Children.Remove(content2);
            }
        }
        public static void OpenPage(Type page)
        {
            try
            {
                if (content2.Content as Page != null)
                {
                    PreviousPage = (content2.Content as Page).NavigationService.Content as Page;
                }
                
                if (page != null)
                {
                    try
                    {
                        if (ReturnParam != null && !SkipParam)
                        {
                            OpenPage(page, ReturnParam);                    
                        }
                        else
                        {
                            SkipParam = false;
                            Page toPage = (Page)Activator.CreateInstance(page);
                            content2.Navigate(toPage);
                            Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been opened this page: {page.Name}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteError(ex);

                        try
                        {
                            SkipParam = false;
                            Page toPage = (Page)Activator.CreateInstance(page);
                            content2.Navigate(toPage);
                            Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been opened this page: {page.Name}.");

                        }
                        catch (Exception ex2)
                        {
                            Debug.WriteError(ex2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurs while redirecting to the page. Please restart the program if this message still appears", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteError(ex);
                throw;
            }
        }

        public static void OpenPageWithParameters(Type page, object param1, object param2)
        {
            try
            {
                if (content2.Content as Page != null)
                {
                    PreviousPage = (content2.Content as Page).NavigationService.Content as Page;
                }

                if (page != null)
                {
                    ConstructorInfo constructor = page.GetConstructor(new Type[] { param1.GetType(), param2.GetType() });

                    if (constructor != null)
                    {
                        object[] constructorParameters = new object[] { param1, param2 };

                        Page toPage = (Page)constructor.Invoke(constructorParameters);
                        content2.Navigate(toPage);
                        Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has opened this page: {page.Name}.");
                    }
                    else
                    {
                        MessageBox.Show($"Constructor with parameters not found for page: {page.Name}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while redirecting to the page. Please restart the program if this message still appears", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteError(ex);
                throw;
            }
        }

        public static void OpenPage(Type Page, object constructorForPage)
        {
            try
            {
                PreviousPage = (content2.Content as Page).NavigationService.Content as Page;
                //Not i but god know what happened here - 2024 04 21
                Type[] constructorParameterTypes = new Type[] { constructorForPage.GetType() };
                ConstructorInfo constructor = Page.GetConstructor(constructorParameterTypes);

                object[] constructorParameters = new object[] { constructorForPage };

                Page ToPage = (Page)constructor.Invoke(constructorParameters); //Ha erre kapsz hibát az azért van mert konstruktort akarsz átadni ott ahol nem kéne
                content2.Navigate(ToPage);
                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has been opened this page: {Page.Name}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurs while redirecting to the page. Please restart the program if this message still appears", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteError(ex);
                throw;
            }
        }

        public static Type GetTypeByName(string typeName)
        {
            try
            {
                Type type = Type.GetType(typeName);

                if (type == null)
                {
                    type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.Name == typeName);
                }
               
                return type;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not load the page!", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteError(ex);
                throw;
            }
        }
    }
}
