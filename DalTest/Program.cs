using Dal;
using DalApi;
using DO;

namespace DalTest

    //NEED TO ADD EXEPTIONS
{
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        enum MenuOptions
        {
            Exit = 0,                 
            VolunteerSubMenu = 1,             
            CallSubMenu = 2, 
            AssignmentSubMenu = 3,
            Initialize = 5,
            ShowAll = 6,
            SubMenuFor = 7,
            Reset = 8
        }
        enum SubMenuOptions
        {
            Exit = 0,
            AddObject = 1,
            ShowObject = 2,
            ShowList = 3,
            Update = 4,
            DeleteObject = 5,
            DeleteAll = 6
        }

        static void Main(string[] args)
        {
        int conditon;
            do
            {
                Console.WriteLine("Enter a number: \n" +
                "0 - exit the menu \n" +
                "1 - enter volunteer sub menu \n" +
                "2 - enter call sub menu \n" +
                "3 - enter assignment sub menu");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)MenuOptions.VolunteerSubMenu:
                           { 
                                volunteerMenu();
                                break;
                           }
                        case (int)MenuOptions.CallSubMenu:
                            {
                                assignmentMenu();
                                break;
                            }
                        case (int)MenuOptions.Initialize:
                            { break; }
                        case (int)MenuOptions.ShowAll:
                            { break; }
                        case (int)MenuOptions.SubMenuFor:
                            { break; }
                        case (int)MenuOptions.Reset:
                            { break; }
                        case (int)MenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

        }

        private static void volunteerMenu()
        {
            int conditon;
            do
            {
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {
                                Volunteer temp = new Volunteer();
                                
                                s_dalVolunteer.Create(temp);
                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                break;
                            }



                        case (int)SubMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

            return;
        }

        private Volunteer newVolunteer()
        {
            /* Volunteer volunteer = new Volunteer();

        // Receive the volunteer's details
        Console.WriteLine("Enter the Volunteer ID:");
        volunteer.Id = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the Volunteer Full Name:");
        volunteer.FullName = Console.ReadLine();

        Console.WriteLine("Enter the Volunteer Email:");
        volunteer.Email = Console.ReadLine();

        Console.WriteLine("Enter the Volunteer Password:");
        volunteer.Password = Console.ReadLine();

        Console.WriteLine("Enter the Volunteer Full Address:");
        volunteer.FullAddress = Console.ReadLine();

        Console.WriteLine("Enter the Volunteer Latitude:");
        volunteer.Latitude = double.Parse(Console.ReadLine());

        Console.WriteLine("Enter the Volunteer Longitude:");
        volunteer.Longitude = double.Parse(Console.ReadLine());

        Console.WriteLine("Is the volunteer active? (true/false):");
        volunteer.IsActive = bool.Parse(Console.ReadLine());

        return volunteer; // Return the new volunteer object*/

            return temp;
        }
        private static void assignmentMenu()
        {

            int conditon;
            do
            {
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {

                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                break;
                            }



                        case (int)SubMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

            return;
        }
        private static void callMenu()
        {

            int conditon;
            do
            {
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {

                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                break;
                            }



                        case (int)SubMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
            while (conditon != 0);

            return;
        }
    }
}
