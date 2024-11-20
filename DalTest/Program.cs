using System;
using Dal;
using DalApi;
using DO;

namespace DalTest

    
{
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
 
        static void Main(string[] args)
        {
        int conditon;
            do
            {
                Console.WriteLine();
                Console.WriteLine("0 - exit the menu \n" +
                "1 - enter volunteer sub menu \n" +
                "2 - enter call sub menu \n" +
                "3 - enter assignment sub menu \n" +
                "5 - initialize values \n" +
                "6 - show all the data \n" +
                "7 - enter config sub menu \n" +
                "8 - reset");
                Console.WriteLine("Enter a number:");
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
                                callMenu();
                                break;
                            }
                       case (int)MenuOptions.AssignmentSubMenu:
                            {
                                assignmentMenu();
                                break;
                            }
                        case (int)MenuOptions.Initialize:
                            {
                                Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                                
                                break;
                            }
                        case (int)MenuOptions.ShowAll:
                            {
                                Console.WriteLine("All the Volunteers:");
                                printAllVolunteers();
                                Console.WriteLine("All the Calls:");
                                printAllCalls();
                                Console.WriteLine("All the Assignments:");
                                printAllAssignments();
                                break;
                            }
                        case (int)MenuOptions.ConfigSubMenu:
                            {
                                configMenu();
                                break; 
                            }
                        case (int)MenuOptions.Reset:
                            {
                                s_dalVolunteer!.DeleteAll();
                                s_dalCall!.DeleteAll();
                                s_dalAssignment!.DeleteAll();
                                s_dalConfig!.Reset();
                               

                                break;
                            }
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
        /// <summary>
        /// This is the sub menu of config
        /// </summary>
        private static void configMenu()
        {
            int conditon;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Config Sub-menu: \n" +
            "0 - Back to main menu \n" +
            "1 - Add Minutes to the Clock \n" +
            "2 - Add Hours to the Clock \n" +
            "3 - Add Days to the Clock\n" +
            "4 - Show current Clock value \n" +
            "5 - Set current value \n" +
            "6 - Show current value\n" +
            "7 - Reset   ");
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)ConfigSumMenuOptions.AddMinuteToClock:
                            {
                                Console.WriteLine("Enter the amount of minutes to add to the clock");
                                int min = int.Parse(Console.ReadLine());
                                s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(min);
                                Console.WriteLine(s_dalConfig.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.AddHourToClock:
                            {
                                Console.WriteLine("Enter the amount of hours to add to the clock");
                                int hour = int.Parse(Console.ReadLine());
                                s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(hour);
                                Console.WriteLine(s_dalConfig.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.AddDayToClock:
                            {
                                Console.WriteLine("Enter the amount of days to add to the clock");
                                int day = int.Parse(Console.ReadLine());
                                s_dalConfig!.Clock = s_dalConfig.Clock.AddDays(day);
                                Console.WriteLine(s_dalConfig.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.ShowCurrentClockValue:
                            {
                                Console.WriteLine(s_dalConfig!.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.SetCurrentValue:
                            {
                                try
                                {
                                    setConfigurationVariable();
                                }
                                catch (Exception ex)
                                {

                                    Console.WriteLine($"Error: {ex.Message}"); 
                                }
                                break;
                            }
                        case (int)ConfigSumMenuOptions.ShowCurrentValue:
                            {
                                try
                                {
                                    getConfigurationVariable();
                                }
                                catch (Exception ex)
                                {

                                    Console.WriteLine($"Error: {ex.Message}");
                                }
                                break;
                            }
                        case (int)ConfigSumMenuOptions.Reset:
                            {
                                s_dalConfig!.Reset();
                                break;
                            }
                        case (int)ConfigSumMenuOptions.Exit:
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
      
        /// <summary>
        /// This is the sub menu of volunteer
        /// </summary>
        private static void volunteerMenu()
        {
            
            int conditon;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Volunteer Sub-menu: \n" +
            "0 - Back to main menu \n" +
            "1 - Add Volunteer \n" +
            "2 - Show Volunteer \n" +
            "3 - Show All Volunteers \n" +
            "4 - Update Volunteer \n" +
            "5 - Delete Volunteer \n" +
            "6 - Delete All Volunteers");
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {
                                addVolunteer();
                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                Console.WriteLine("Enter the Volunteer ID: ");
                                int id = int.Parse(Console.ReadLine());
                                printVolunteer(id);
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                printAllVolunteers();
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                Console.WriteLine("Enter the Volunteer ID: ");
                                int id = int.Parse(Console.ReadLine());
                                updateVolunteer(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                Console.WriteLine("Enter the Volunteer ID: ");
                                int id = int.Parse(Console.ReadLine());
                                deleteVolunteer(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                deleteAllVolunteers();
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

        /// <summary>
        /// This function deletes all the volunteers
        /// </summary>
        private static void deleteAllVolunteers()
        {
            try
            {
                s_dalVolunteer!.DeleteAll();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}"); ;
            }
            
        }

        /// <summary>
        /// Get an id of a volunteer and deletes him
        /// </summary>
        /// <param name="id">The id of the volunteer we want to delete</param>
        private static void deleteVolunteer(int id)
        {
            try
            {
                s_dalVolunteer!.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); ;
            }
            
        }
        /// <summary>
        /// Get an id of a volunteer and updates him
        /// </summary>
        /// <param name="id">The id of the volunteer we want to update</param>
        private static void updateVolunteer(int id)
        {
            printVolunteer(id);
            //s_dalVolunteer.Update(temp); if we dont need to revice new values, then this line is enough
            //delete the volunteer
            deleteVolunteer(id);
            //recive new values and add them
            addVolunteer();
        }
       /// <summary>
       /// Creates a volunteer using the function createVolunteer, and adds him to the list
       /// </summary>
        private static void addVolunteer()
        {
            //create new volunteer with the func and try to add it to the list
            Volunteer temp = createVolunteer();
            try
            {
                s_dalVolunteer!.Create(temp);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");

            }
        }

        /// <summary>
        /// Creates new volunteer
        /// </summary>
        /// <returns>volunteer</returns>
        private static Volunteer createVolunteer()
        {
            int id;
            string fullName,phoneNumber, email, password, fullAddress;
            bool isActive;

            // Receive the volunteer's details into variables
            Console.WriteLine("Enter the Volunteer ID:");
            id = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the Volunteer Full Name:");
            fullName = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Phone number:");
            phoneNumber = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Email:");
            email = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Password:");
            password = Console.ReadLine();

            Console.WriteLine("Enter the Volunteer Full Address:");
            fullAddress = Console.ReadLine();

            Console.WriteLine("Is the volunteer active? (true/false):");
            isActive = bool.Parse(Console.ReadLine());

            return new Volunteer
            {
                Id = id,
                FullName = fullName,
                MobilePhone = phoneNumber,
                Email = email,
                Password = password,
                FullAddress = fullAddress,
                Role = Role.Volunteer,
                IsActive = isActive
            };
        }
        /// <summary>
        /// Get an id of a volunteer and prints him values
        /// </summary>
        /// <param name="id">The id of the volunteer we want to print</param>
        private static void printVolunteer(int id)
        {
            try
            {
                Console.WriteLine(" ");
                Volunteer vol = s_dalVolunteer!.Read(id);
                Console.WriteLine($"ID: {vol.Id}");
                Console.WriteLine($"Full Name: {vol.FullName}");
                Console.WriteLine($"Phone Number: {vol.MobilePhone}");
                Console.WriteLine($"Email: {vol.Email}");
                Console.WriteLine($"Password: {vol.Password}");
                Console.WriteLine($"Address: {vol.FullAddress}");
                Console.WriteLine($"Latitude: {vol.Latitude}");
                Console.WriteLine($"Longitude: {vol.Longitude}");
                Console.WriteLine($"Role: {vol.Role}");
                if (vol.IsActive)
                    Console.WriteLine("Is Active");
                else
                    Console.WriteLine("Is Not Active");

                Console.WriteLine(" ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Prints all the volunteers by sending each one to the function printVolunteer
        /// </summary>
        private static void printAllVolunteers()
        {
            List<Volunteer> temp = s_dalVolunteer!.ReadAll();
            foreach (var vol in temp)
            {
                printVolunteer(vol.Id);
            }
        }
        /// <summary>
        /// Sub menu of Assignment
        /// </summary>
        private static void assignmentMenu()
        {
         
            int conditon;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Assignment Sub-menu: \n" +
                    "0 - Back to main menu \n" +
                    "1 - Add Assignment \n" +
                    "2 - Show Assignment \n" +
                    "3 - Show All Assignments \n" +
                    "4 - Update Assignment \n" +
                    "5 - Delete Assignment \n" +
                    "6 - Delete All Assignments");
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {
                                addAssignment();
                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                Console.WriteLine("Enter the Assignment ID: ");
                                int id = int.Parse(Console.ReadLine());
                                printAssignment(id);
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                printAllAssignments();
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                int id = int.Parse(Console.ReadLine());
                                updateAssignment(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                int id = int.Parse(Console.ReadLine());
                                deleteAssignment(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                deleteAllAssignments();
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
        /// <summary>
        /// Get an id of an assignment and updates it
        /// </summary>
        /// <param name="id">The id of the assignment we want to update</param>
        private static void updateAssignment(int id)
        {
            printAssignment(id);
            deleteAssignment(id);
            addAssignment();
        }
        /// <summary>
        /// Prints all the assignments by sending each one to the function printVolunteer
        /// </summary>
        private static void printAllAssignments()
        {
            List<Assignment> temp = s_dalAssignment!.ReadAll();
            foreach (var ass in temp)
            {
                printAssignment(ass.Id);
            }
        }
        /// <summary>
        /// Get an id of an assignment and prints it
        /// </summary>
        /// <param name="id">The id of the assignment we want to print</param>
        private static void printAssignment(int id)
        {
            try
            {
                Console.WriteLine();
                Assignment assignment = s_dalAssignment!.Read(id);
                Console.WriteLine("Assignment Details:");
                Console.WriteLine($"ID: {assignment.Id}");
                Console.WriteLine($"Call ID: {assignment.CallId}");
                Console.WriteLine($"Volunteer ID: {assignment.VolunteerId}");
                Console.WriteLine($"Admission Time: {assignment.AdmissionTime}");
                Console.WriteLine($"Actual End Time: {(assignment.ActualEndTime.HasValue ? assignment.ActualEndTime.ToString() : "Not ended")}");
                Console.WriteLine($"Treatment End Type: {(assignment.TreatmentEndType.HasValue ? assignment.TreatmentEndType.ToString() : "None")}");
                Console.WriteLine(); // For better readability
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }
        /// <summary>
        /// Creates an assignment using the function createAssignment and adds it to the list
        /// </summary>
        private static void addAssignment()
        {
            Assignment temp = createAssignment();
            try
            {
                s_dalAssignment!.Create(temp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Recives values for an assignment and returns it
        /// </summary>
        /// <returns>new Assignment</returns>
        private static Assignment createAssignment()
        {
            // Receive input for each parameter
            Console.WriteLine("Enter the Call ID:");
            int callId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the Volunteer ID:");
            int volunteerId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the Admission Time (or press Enter for current date and time):");
            string admissionTimeInput = Console.ReadLine();
            DateTime admissionTime = string.IsNullOrWhiteSpace(admissionTimeInput)
                ? DateTime.Now
                : DateTime.Parse(admissionTimeInput);

            Console.WriteLine("Enter the Actual End Time (or press Enter if not ended):");
            string actualEndTimeInput = Console.ReadLine();
            DateTime? actualEndTime = string.IsNullOrWhiteSpace(actualEndTimeInput)
                ? null
                : DateTime.Parse(actualEndTimeInput);

            Console.WriteLine("Enter the Treatment End Type (or press Enter for none):");
            string treatmentEndTypeInput = Console.ReadLine();
            TreatmentEndType? treatmentEndType = string.IsNullOrWhiteSpace(treatmentEndTypeInput)
                ? null
                : Enum.Parse<TreatmentEndType>(treatmentEndTypeInput, true); // Assuming TreatmentEndType is an enum

            // Create and return the Assignment object
            return new Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                AdmissionTime = admissionTime,
                ActualEndTime = actualEndTime,
                TreatmentEndType = treatmentEndType
            };

        }
       /// <summary>
       /// Deletes all the assignments
       /// </summary>
        private static void deleteAllAssignments()
        {
            try
            {
                s_dalAssignment!.DeleteAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); ;
            }
        }
       /// <summary>
       /// Recives an id and deletes the assignment with this id
       /// </summary>
       /// <param name="id">the id of the assignment we want to delete</param>
        private static void deleteAssignment(int id)
        {
            try
            {
                s_dalAssignment!.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); ;
            }
        }
        /// <summary>
        /// Call sub menu
        /// </summary>
        private static void callMenu()
        {
            
            int conditon;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Call Sub-menu: \n" +
                  "0 - Back to main menu \n" +
                  "1 - Add Call \n" +
                  "2 - Show Call \n" +
                    "3 - Show All Calls \n" +
                 "4 - Update Call \n" +
                 "5 - Delete Call \n" +
                 "6 - Delete All Calls");
                Console.WriteLine("Enter a number:");
                string input = Console.ReadLine();

                // Validate the input
                if (int.TryParse(input, out conditon))
                {
                    switch (conditon)
                    {
                        case (int)SubMenuOptions.AddObject:
                            {
                                addCall();
                                break;
                            }
                        case (int)SubMenuOptions.ShowObject:
                            {
                                int id = int.Parse(Console.ReadLine());
                                printCall(id);
                                break;
                            }
                        case (int)SubMenuOptions.ShowList:
                            {
                                printAllCalls();
                                break;
                            }
                        case (int)SubMenuOptions.Update:
                            {
                                int id = int.Parse(Console.ReadLine());
                                updateCall(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                int id = int.Parse(Console.ReadLine());
                                deleteCall(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteAll:
                            {
                                deleteAllCalls();
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
        /// <summary>
        /// Recives values for a call and returns it
        /// </summary>
        /// <returns>new Call</returns>
        private static Call createCall()
        {
            // Receive input for each parameter
            Console.WriteLine("Enter the Call Type (FoodPackaging, VolunteeringWithChildren, etc. or press Enter for Undefined):");
            string callTypeInput = Console.ReadLine();
            CallType callType = string.IsNullOrWhiteSpace(callTypeInput)
                ? CallType.Undefined
                : Enum.Parse<CallType>(callTypeInput, true);

            Console.WriteLine("Enter the Call Description (or press Enter to skip):");
            string? description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description)) description = null;

            Console.WriteLine("Enter the Call Full Address:");
            string fullAddress = Console.ReadLine();

            Console.WriteLine("Enter the Opening Time (or press Enter for current date and time):");
            string openingTimeInput = Console.ReadLine();
            DateTime openingTime = string.IsNullOrWhiteSpace(openingTimeInput)
                ? DateTime.Now
                : DateTime.Parse(openingTimeInput);

            Console.WriteLine("Enter the Maximum Completion Time (or press Enter for none):");
            string maxCompletionTimeInput = Console.ReadLine();
            DateTime? maxCompletionTime = string.IsNullOrWhiteSpace(maxCompletionTimeInput)
                ? null
                : DateTime.Parse(maxCompletionTimeInput);

            // Create and return the Call object
            return new Call
            {
                CallType = callType,
                Description = description,
                FullAddress = fullAddress,
                OpeningTime = openingTime,
                MaxCompletionTime = maxCompletionTime
            };
        }
        /// <summary>
        /// Get an id of a call and updates it
        /// </summary>
        /// <param name="id">The id of the call we want to update</param>
        private static void updateCall(int id)
        {
            printCall(id);
            deleteCall(id);
            addCall();
        }
        /// <summary>
        /// Prints all the calls by sending each one to the function printCall
        /// </summary>
        private static void printAllCalls()
        {
            List<Call> temp = s_dalCall.ReadAll();
            foreach (var call in temp)
            {
                printCall(call.Id);
            }
        }
        /// <summary>
        /// Get an id of a call and prints it
        /// </summary>
        /// <param name="id">The id of the assignment we want to print</param>
        static void printCall(int id)
        {
            try
            {
                Console.WriteLine();
                Call call = s_dalCall!.Read(id);
                Console.WriteLine("Call Details:");
                Console.WriteLine($"ID: {call.Id}");
                Console.WriteLine($"Call Type: {call.CallType}");
                Console.WriteLine($"Description: {call.Description ?? "No description provided"}");
                Console.WriteLine($"Full Address: {call.FullAddress}");
                Console.WriteLine($"Latitude: {call.Latitude}");
                Console.WriteLine($"Longitude: {call.Longitude}");
                Console.WriteLine($"Opening Time: {call.OpeningTime}");
                Console.WriteLine($"Maximum Completion Time: {(call.MaxCompletionTime.HasValue ? call.MaxCompletionTime.ToString() : "No maximum time set")}");
                Console.WriteLine(); // Add an empty line for better readability
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
            }    
        }

        /// <summary>
        /// Creates a call using the function createCall and adds it to the list
        /// </summary>
        private static void addCall()
        {
            Call temp = createCall();
            try
            {
                s_dalCall!.Create(temp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
       /// <summary>
       /// Deletes all the calls
       /// </summary>
        private static void deleteAllCalls()
        {
            try
            {
                s_dalCall!.DeleteAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); ;
            }
        }
        /// <summary>
        /// Get an id of a call and deletes it
        /// </summary>
        /// <param name="id">The id of the call we want to delete</param>
        private static void deleteCall(int id)
        {
            try
            {
                s_dalCall!.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); ;
            }
        }
        /// <summary>
        /// The user can choose to change either the Clock or the RiskRange
        /// </summary>
        /// <exception cref="FormatException">if the user entered wrong time format</exception>
        public static void setConfigurationVariable()
        {
            Console.WriteLine("Choose the configuration variable to update:");
            Console.WriteLine("1. Clock");
            Console.WriteLine("2. RiskRange");
            Console.Write("Please choose an option: ");
            ConfigOptions configInput;
            ConfigOptions.TryParse(Console.ReadLine(), out configInput);
            switch (configInput)
            {
                case ConfigOptions.Clock:
                    Console.Write("Enter the new value for Clock (yyyy-MM-dd HH:mm:ss): ");
                    DateTime newClock;
                    if (!DateTime.TryParse(Console.ReadLine(), out newClock)) throw new FormatException("Invalid date format.");
                    {
                        s_dalConfig!.Clock = newClock;
                        Console.WriteLine("Clock updated.");
                    }
                    break;

                case ConfigOptions.RiskRange:
                    Console.Write("Enter the new value for RiskRange (hh:mm:ss): ");
                    TimeSpan newRiskRange;
                    if (!TimeSpan.TryParse(Console.ReadLine(), out newRiskRange)) throw new FormatException("Invalid date format.");
                    {
                        s_dalConfig!.RiskRange = newRiskRange;
                        Console.WriteLine("RiskRange updated.");
                    }
                    break;

                default:
                    {
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                    }
            }
        }

        /// <summary>
        /// The user can choose to see either the Clock or the RiskRange
        /// </summary>
        public static void getConfigurationVariable()
        {
            Console.WriteLine("Choose a configuration variable to view:");
            Console.WriteLine("1. Clock");
            Console.WriteLine("2. RiskRange");
            Console.Write("Enter your choice: ");
            ConfigOptions choice;
            ConfigOptions.TryParse(Console.ReadLine(), out choice);

            switch (choice)
            {
                case ConfigOptions.Clock:
                    Console.WriteLine($"Clock: {s_dalConfig?.Clock}");
                    break;
                case ConfigOptions.RiskRange:
                    Console.WriteLine($"RiskRange: {s_dalConfig?.RiskRange}");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                    break;
            }
        }
       

    }
}
