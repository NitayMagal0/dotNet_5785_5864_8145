using System;
using System.Diagnostics;
using Dal;
using DalApi;
using DO;

namespace DalTest;



internal class Program
{
    private static readonly IDal s_dal = new DalList();
    //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    //private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
    //private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    //private static ICall? s_dalCall = new CallImplementation(); //stage 1

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
                            Initialization.Do(s_dal);

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
                            s_dal.Volunteer!.DeleteAll();
                            s_dal.Call!.DeleteAll();
                            s_dal.Assignment!.DeleteAll();
                            s_dal.Config!.Reset();


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
                conditon = -1;
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
                try
                {
                    switch (conditon)
                    {
                        case (int)ConfigSumMenuOptions.AddMinuteToClock:
                            {
                                Console.WriteLine("Enter the amount of minutes to add to the clock");
                                if (!int.TryParse(Console.ReadLine(), out int min))
                                {
                                    throw new DalInvalidFormatException(
                                        "Invalid format: Please enter a valid number of minutes.");
                                }

                                s_dal.Config!.Clock = s_dal.Config.Clock.AddMinutes(min);
                                Console.WriteLine(s_dal.Config.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.AddHourToClock:
                            {
                                Console.WriteLine("Enter the amount of hours to add to the clock");
                                if (!int.TryParse(Console.ReadLine(), out int hour))
                                {
                                    throw new DalInvalidFormatException(
                                        "Invalid format: Please enter a valid number of hours.");
                                }

                                s_dal.Config!.Clock = s_dal.Config.Clock.AddHours(hour);
                                Console.WriteLine(s_dal.Config.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.AddDayToClock:
                            {
                                Console.WriteLine("Enter the amount of days to add to the clock");
                                if (!int.TryParse(Console.ReadLine(), out int day))
                                {
                                    throw new DalInvalidFormatException(
                                        "Invalid format: Please enter a valid number of days.");
                                }

                                s_dal.Config!.Clock = s_dal.Config.Clock.AddDays(day);
                                Console.WriteLine(s_dal.Config.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.ShowCurrentClockValue:
                            {
                                Console.WriteLine(s_dal.Config!.Clock);
                                break;
                            }
                        case (int)ConfigSumMenuOptions.SetCurrentValue:
                            {

                                setConfigurationVariable();
                                break;
                            }
                        case (int)ConfigSumMenuOptions.ShowCurrentValue:
                            {
                                getConfigurationVariable();
                                break;
                            }
                        case (int)ConfigSumMenuOptions.Reset:
                            {
                                s_dal.Config!.Reset();
                                break;
                            }
                        case (int)ConfigSumMenuOptions.Exit:
                            //just break and the condition below will exit the loop
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    conditon = -1;
                }

            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                conditon = -1;
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
                try
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
                                int id = GetValidatedIdFromConsole();
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
                                int id = GetValidatedIdFromConsole();
                                updateVolunteer(id);
                                break;
                            }
                        case (int)SubMenuOptions.DeleteObject:
                            {
                                Console.WriteLine("Enter the Volunteer ID: ");
                                int id = GetValidatedIdFromConsole();
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    conditon = -1;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                conditon = -1;
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
            s_dal.Volunteer!.DeleteAll();
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
            s_dal.Volunteer!.Delete(id);
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
            s_dal.Volunteer!.Create(temp);
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
        int id = 0;
        string fullName = null, phoneNumber = null, email = null, password = null, fullAddress = null;
        bool isActive = false, flag = true;
        while (flag)
        {
            try
            {
                // Receive the volunteer's details into variables
                Console.WriteLine("Enter the Volunteer ID: ");
                id = GetValidatedIdFromConsole();
                Console.WriteLine("Enter the Volunteer Full Name:");
                fullName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    throw new DalInvalidFormatException("Name can't be null or empty");
                }
                Console.WriteLine("Enter the Volunteer Phone number:");
                phoneNumber = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    throw new DalInvalidFormatException("Phone number can't be null or empty");
                }
                Console.WriteLine("Enter the Volunteer Email:");
                email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new DalInvalidFormatException("Email can't be null or empty");
                }
                Console.WriteLine("Enter the Volunteer Password:");
                password = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new DalInvalidFormatException("Password can't be null or empty");
                }
                Console.WriteLine("Enter the Volunteer Full Address:");
                fullAddress = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fullAddress))
                {
                    throw new DalInvalidFormatException("Address can't be null or empty");
                }
                Console.WriteLine("Is the volunteer active? (true/false):");
                if (!bool.TryParse(Console.ReadLine(), out isActive))
                {
                    throw new DalInvalidFormatException("Input for 'Is the volunteer active?' is invalid!");
                }
                flag = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
                flag = true;
            }
        }

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
            Volunteer vol = s_dal.Volunteer!.Read(id) ?? throw new DalNullReferenceException("Volunteer not found."); ;
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
        List<Volunteer> temp = s_dal.Volunteer!.ReadAll().ToList();
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
                try
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
                            int id = GetValidatedIdFromConsole();
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
                            Console.WriteLine("Enter the Assignment ID: ");
                            int id = GetValidatedIdFromConsole();
                            updateAssignment(id);
                            break;
                        }
                        case (int)SubMenuOptions.DeleteObject:
                        {
                            Console.WriteLine("Enter the Assignment ID: ");
                            int id = GetValidatedIdFromConsole();
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
                catch(Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    conditon = -1;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                conditon = -1;
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
        List<Assignment> temp = s_dal.Assignment!.ReadAll().ToList();
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
            Assignment assignment = s_dal.Assignment!.Read(id) ?? throw new DalNullReferenceException("Assignment not found.");
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
            s_dal.Assignment!.Create(temp);
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
        int callId = 0, volunteerId = 0;
        bool flag = false;
        DateTime admissionTime = DateTime.Now;
        DateTime? actualEndTime = null;
        TreatmentEndType? treatmentEndType = null;
        do
        {
            try
            {

                // Receive input for each parameter
                Console.WriteLine("Enter the Call ID:");
                callId = GetValidatedIdFromConsole();

                Console.WriteLine("Enter the Volunteer ID:");
                volunteerId = GetValidatedIdFromConsole();

                Console.WriteLine("Enter the Admission Time (or press Enter for current date and time):");
                string admissionTimeInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(admissionTimeInput))
                {
                    admissionTime = DateTime.Now;
                }
                else if (!DateTime.TryParse(admissionTimeInput, out admissionTime))
                {
                    throw new DalInvalidFormatException("Admission Time is invalid!");
                }

                Console.WriteLine("Enter the Actual End Time (or press Enter if not ended):");
                string actualEndTimeInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(actualEndTimeInput))
                {
                    if (!DateTime.TryParse(actualEndTimeInput, out DateTime parsedActualEndTime))
                    {
                        throw new DalInvalidFormatException("Actual End Time is invalid!");
                    }
                    actualEndTime = parsedActualEndTime;
                }
                Console.WriteLine("Enter the Treatment End Type (or press Enter for none):");
                string treatmentEndTypeInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(treatmentEndTypeInput))
                {
                    if (!Enum.TryParse<TreatmentEndType>(treatmentEndTypeInput, true, out TreatmentEndType parsedTreatmentEndType))
                    {
                        throw new FormatException("Treatment End Type is invalid!");
                    }
                    treatmentEndType = parsedTreatmentEndType;
                }
                flag = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
                flag = true;
            }
        } while (flag);

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
            s_dal.Assignment!.DeleteAll();
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
            s_dal.Assignment!.Delete(id);
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
                            Console.WriteLine("Enter the Call ID: ");
                            int id = GetValidatedIdFromConsole();
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
                            Console.WriteLine("Enter the Call ID: ");

                            int id = GetValidatedIdFromConsole();
                            updateCall(id);
                            break;
                        }
                    case (int)SubMenuOptions.DeleteObject:
                        {
                            Console.WriteLine("Enter the Call ID: ");
                            int id = GetValidatedIdFromConsole();
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
                conditon = -1;
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
        CallType callType = CallType.Undefined;
        string? description = null;
        bool flag = true;
        DateTime openingTime = DateTime.Now;
        string fullAddress = null;
        DateTime? maxCompletionTime = null;
        do
        {
            try
            {
                // Receive input for each parameter
                Console.WriteLine("Enter the Call Type (FoodPackaging, VolunteeringWithChildren, etc. or press Enter for Undefined):");
                string callTypeInput = Console.ReadLine();
                callType = CallType.Undefined;
                if (!string.IsNullOrWhiteSpace(callTypeInput))
                {
                    if (!Enum.TryParse<CallType>(callTypeInput, true, out callType))
                    {
                        throw new ArgumentException($"Invalid call type: {callTypeInput}");
                    }
                }

                Console.WriteLine("Enter the Call Description (or press Enter to skip):");
                description = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(description)) description = null;

                Console.WriteLine("Enter the Call Full Address:");
                fullAddress = Console.ReadLine();

                Console.WriteLine("Enter the Opening Time (or press Enter for current date and time):");
                string openingTimeInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(openingTimeInput))
                {
                    openingTime = DateTime.Now;
                }
                else if (!DateTime.TryParse(openingTimeInput, out openingTime))
                {
                    throw new ArgumentException($"Invalid date time format: {openingTimeInput}");
                }

                Console.WriteLine("Enter the Maximum Completion Time (or press Enter for none):");
                string maxCompletionTimeInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(maxCompletionTimeInput))
                {
                    if (!DateTime.TryParse(maxCompletionTimeInput, out DateTime parsedDate))
                    {
                        throw new ArgumentException($"Invalid date time format: {maxCompletionTimeInput}");
                    }

                    maxCompletionTime = parsedDate;
                }
                flag = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
            }
        } while (flag);

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
        List<Call> temp = s_dal!.Call.ReadAll().ToList();
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
            Call call = s_dal!.Call!.Read(id) ?? throw new DalNullReferenceException("Call not found."); ;
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
            s_dal!.Call!.Create(temp);
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
            s_dal!.Call!.DeleteAll();
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
            s_dal!.Call!.Delete(id);
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
                if (!DateTime.TryParse(Console.ReadLine(), out newClock)) throw new DalInvalidFormatException("Invalid date format.");
                {
                    s_dal!.Config!.Clock = newClock;
                    Console.WriteLine("Clock updated.");
                }
                break;

            case ConfigOptions.RiskRange:
                Console.Write("Enter the new value for RiskRange (hh:mm:ss): ");
                TimeSpan newRiskRange;
                if (!TimeSpan.TryParse(Console.ReadLine(), out newRiskRange)) throw new DalInvalidFormatException("Invalid date format.");
                {
                    s_dal!.Config!.RiskRange = newRiskRange;
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
                Console.WriteLine($"Clock: {s_dal!.Config?.Clock}");
                break;
            case ConfigOptions.RiskRange:
                Console.WriteLine($"RiskRange: {s_dal!.Config?.RiskRange}");
                break;
            default:
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                break;
        }
    }
    /// <summary>
    /// Tries to parse the input from the console as an integer.
    /// Throws a DalInvalidFormatException if the input is not a valid integer.
    /// </summary>
    /// <returns>The parsed integer.</returns>
    /// <exception cref="DalInvalidFormatException">Thrown when the input is not a valid integer.</exception>
    private static int GetValidatedIdFromConsole()
    {
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            throw new DalInvalidFormatException("Invalid format: Please enter a valid ID.");
        }
        return id;
    }

}


