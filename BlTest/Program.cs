using BlApi;
using System;
using BO;

namespace BlTest
{
    class Program
    {
        static readonly IBl s_bl = Factory.Get();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Admin Operations");
                Console.WriteLine("2. Volunteer Operations");
                Console.WriteLine("3. Call Operations");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int mainOption) || mainOption < 0 || mainOption > 3)
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    continue;
                }

                switch (mainOption)
                {
                    case 1:
                        AdminOperations();
                        break;
                    case 2:
                        VolunteerOperations();
                        break;
                    case 3:
                        CallOperations();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void AdminOperations()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Admin Operations:");
                Console.WriteLine("1. Reset DB");
                Console.WriteLine("2. Initialize DB");
                Console.WriteLine("3. Forward Clock");
                Console.WriteLine("4. Get Clock");
                Console.WriteLine("5. Get Max Range");
                Console.WriteLine("6. Set Max Range");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int adminOption) || adminOption < 0 || adminOption > 6)
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    continue;
                }

                try
                {
                    switch (adminOption)
                    {
                        case 1:
                            s_bl.Admin.ResetDB();
                            Console.WriteLine("Database reset successfully.");
                            break;
                        case 2:
                            s_bl.Admin.InitializeDB();
                            Console.WriteLine("Database initialized successfully.");
                            break;
                        case 3:
                            Console.Write("Enter time unit to forward (e.g., HOUR): ");
                            if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.ForwardClock(timeUnit);
                                Console.WriteLine("Clock forwarded successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid time unit.");
                            }
                            break;
                        case 4:
                            Console.WriteLine($"Current Clock: {s_bl.Admin.GetClock()}");
                            break;
                        case 5:
                            Console.WriteLine($"Max Range: {s_bl.Admin.GetMaxRange()}");
                            break;
                        case 6:
                            Console.Write("Enter max range (in format hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan maxRange))
                            {
                                s_bl.Admin.SetMaxRange(maxRange);
                                Console.WriteLine("Max range set successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid time span format.");
                            }
                            break;
                        case 0:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private static void VolunteerOperations()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Volunteer Operations:");
                Console.WriteLine("1. Sign In");
                Console.WriteLine("2. Add Volunteer");
                Console.WriteLine("3. Update Volunteer");
                Console.WriteLine("4. Delete Volunteer");
                Console.WriteLine("5. Get Volunteer Details");
                Console.WriteLine("6. Get Volunteers List");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int volunteerOption) || volunteerOption < 0 || volunteerOption > 6)
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    continue;
                }

                try
                {
                    switch (volunteerOption)
                    {
                        case 1:
                            // Sign In
                            Console.Write("Enter volunteer name: ");
                            string name = Console.ReadLine();
                            Console.Write("Enter password: ");
                            string password = Console.ReadLine();
                            var role = s_bl.Volunteer.SignIn(name, password);
                            Console.WriteLine($"Signed in as {role}");
                            break;
                        case 2:
                            // Add Volunteer
                            var newVolunteer = new BO.Volunteer();
                            Console.WriteLine("Enter volunteer id: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                newVolunteer.Id = id;
                            }
                            Console.Write("Enter volunteer name: ");
                            newVolunteer.FullName = Console.ReadLine();
                            Console.Write("Enter volunteer mobile phone: ");
                            newVolunteer.MobilePhone = Console.ReadLine();
                            Console.Write("Enter volunteer email: ");
                            newVolunteer.Email = Console.ReadLine();
                            Console.Write("Enter volunteer password: ");
                            newVolunteer.Password = Console.ReadLine();
                            Console.Write("Enter volunteer full address: ");
                            newVolunteer.FullAddress = Console.ReadLine();
                            Console.Write("Enter volunteer role (Manager/Volunteer): ");
                            if (Enum.TryParse(Console.ReadLine(), out Role role2))
                            {
                                newVolunteer.Role = role2;
                            }
                            Console.Write("Is the volunteer active? (true/false): ");
                            if (bool.TryParse(Console.ReadLine(), out bool isActive))
                            {
                                newVolunteer.IsActive = isActive;
                            }
                            Console.Write("Enter max distance for call: ");
                            if (double.TryParse(Console.ReadLine(), out double maxDistanceForCall))
                            {
                                newVolunteer.MaxDistanceForCall = maxDistanceForCall;
                            }
                            Console.Write("Enter distance type (AirDistance/WalkingDistance/DrivingDistance): ");
                            if (Enum.TryParse(Console.ReadLine(), out DistanceType distanceType))
                            {
                                newVolunteer.DistanceType = distanceType;
                            }
                            s_bl.Volunteer.AddVolunteer(newVolunteer);
                            Console.WriteLine("Volunteer added successfully.");
                            break;
                        case 3:
                            // Update Volunteer
                            Console.Write("Enter volunteer ID to update: ");
                            if (int.TryParse(Console.ReadLine(), out int updateVolunteerId))
                            {
                                var updateVolunteer = s_bl.Volunteer.GetVolunteerDetails(updateVolunteerId);
                                Console.Write("Enter new volunteer name: ");
                                updateVolunteer.FullName = Console.ReadLine();
                                // Update other properties as needed
                                Console.Write("Enter new volunteer mobile phone: ");
                                updateVolunteer.MobilePhone = Console.ReadLine();
                                Console.Write("Enter new volunteer email: ");
                                updateVolunteer.Email = Console.ReadLine();
                                Console.Write("Enter new volunteer password: ");
                                updateVolunteer.Password = Console.ReadLine();
                                Console.Write("Enter new volunteer full address: ");
                                updateVolunteer.FullAddress = Console.ReadLine();
                                Console.Write("Enter new volunteer role (Manager/Volunteer): ");
                                if (Enum.TryParse(Console.ReadLine(), out Role newRole))
                                {
                                    updateVolunteer.Role = newRole;
                                }
                                Console.Write("Is the volunteer active? (true/false): ");
                                if (bool.TryParse(Console.ReadLine(), out bool newIsActive))
                                {
                                    updateVolunteer.IsActive = newIsActive;
                                }
                                Console.Write("Enter new max distance for call: ");
                                if (double.TryParse(Console.ReadLine(), out double newMaxDistanceForCall))
                                {
                                    updateVolunteer.MaxDistanceForCall = newMaxDistanceForCall;
                                }
                                Console.Write("Enter new distance type (AirDistance/WalkingDistance/DrivingDistance): ");
                                if (Enum.TryParse(Console.ReadLine(), out DistanceType newDistanceType))
                                {
                                    updateVolunteer.DistanceType = newDistanceType;
                                }
                                s_bl.Volunteer.UpdateVolunteer(updateVolunteerId, updateVolunteer);
                                Console.WriteLine("Volunteer updated successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 4:
                            // Delete Volunteer
                            Console.Write("Enter volunteer ID to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteVolunteerId))
                            {
                                s_bl.Volunteer.DeleteVolunteer(deleteVolunteerId);
                                Console.WriteLine("Volunteer deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 5:
                            // Get Volunteer Details
                            Console.Write("Enter volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                                Console.WriteLine();
                                Console.WriteLine(volunteer);
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 6:
                            // Get Volunteers List
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteersList(null, null))
                            {
                                Console.WriteLine();
                                Console.WriteLine(volunteer);
                            }
                            break;
                        case 0:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private static void CallOperations()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Call Operations:");
                Console.WriteLine("1. Add Call");
                Console.WriteLine("2. Update Call");
                Console.WriteLine("3. Delete Call");
                Console.WriteLine("4. Get Call Details");
                Console.WriteLine("5. Get Calls List");
                Console.WriteLine("6. Get Call Counts By Status");
                Console.WriteLine("7. Get Volunteer Closed Calls History");
                Console.WriteLine("8. Get Available Open Calls For Volunteer");
                Console.WriteLine("9. Mark Assignment As Completed");
                Console.WriteLine("10. Cancel Assignment");
                Console.WriteLine("11. Assign Call To Volunteer");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int callOption) || callOption < 0 || callOption > 11)
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    continue;
                }

                try
                {
                    switch (callOption)
                    {
                        case 1:
                            // Add Call
                            var newCall = new BO.Call();
                            // Add other properties as needed
                            Console.Write("Enter call type (Undefined, CleaningShelters, HelpForFamiliesInNeed, FoodPackagingForNeedyFamilies, HospitalVisitsForMoraleBoost): ");
                            if (Enum.TryParse(Console.ReadLine(), out CallType callType))
                            {
                                newCall.CallType = callType;
                            }
                            Console.Write("Enter full address: ");
                            newCall.FullAddress = Console.ReadLine();
                            Console.Write("Enter opening time (yyyy-MM-dd HH:mm:ss): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime openingTime))
                            {
                                newCall.OpeningTime = openingTime;
                            }
                            Console.Write("Enter max completion time (yyyy-MM-dd HH:mm:ss): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime maxCompletionTime))
                            {
                                newCall.MaxCompletionTime = maxCompletionTime;
                            }
                            newCall.CallAssigns = new List<BO.CallAssignInList>();
                            Console.Write("Enter call description: ");
                            newCall.Description = Console.ReadLine();
                            // Add other properties as needed
                            s_bl.Call.AddCall(newCall);
                            Console.WriteLine("Call added successfully.");
                            break;
                        case 2:
                            // Update Call
                            Console.Write("Enter call ID to update: ");
                            if (int.TryParse(Console.ReadLine(), out int updateCallId))
                            {
                                var updateCall = s_bl.Call.GetCallDetails(updateCallId);
                                Console.Write("Enter new call description: ");
                                updateCall.Description = Console.ReadLine();
                                // Update other properties as needed
                                Console.Write("Enter new call type (Undefined, CleaningShelters, HelpForFamiliesInNeed, FoodPackagingForNeedyFamilies, HospitalVisitsForMoraleBoost): ");
                                if (Enum.TryParse(Console.ReadLine(), out CallType newCallType))
                                {
                                    updateCall.CallType = newCallType;
                                }
                                Console.Write("Enter new full address: ");
                                updateCall.FullAddress = Console.ReadLine();
                                Console.Write("Enter new opening time (yyyy-MM-dd HH:mm:ss): ");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime newOpeningTime))
                                {
                                    updateCall.OpeningTime = newOpeningTime;
                                }
                                Console.Write("Enter new max completion time (yyyy-MM-dd HH:mm:ss): ");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime newMaxCompletionTime))
                                {
                                    updateCall.MaxCompletionTime = newMaxCompletionTime;
                                }
                                s_bl.Call.UpdateCall(updateCall);
                                Console.WriteLine("Call updated successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid call ID.");
                            }
                            break;
                        case 3:
                            // Delete Call
                            Console.Write("Enter call ID to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteCallId))
                            {
                                s_bl.Call.DeleteCall(deleteCallId);
                                Console.WriteLine("Call deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid call ID.");
                            }
                            break;
                        case 4:
                            // Get Call Details
                            Console.Write("Enter call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int callId))
                            {
                                var call = s_bl.Call.GetCallDetails(callId);
                                Console.WriteLine(call.ToString());
                            }
                            else
                            {
                                Console.WriteLine("Invalid call ID.");
                            }
                            break;
                        case 5:
                            // Get Calls List
                            foreach (var call in s_bl.Call.GetFilteredAndSortedCalls(null, null, null))
                            {
                                Console.WriteLine(call);
                            }
                            break;
                        case 6:
                            // Get Call Counts By Status
                            var counts = s_bl.Call.GetCallCountsByStatus();
                            Console.WriteLine("Call Counts By Status:");
                            for (int i = 0; i < counts.Length; i++)
                            {
                                Console.WriteLine($"Status {i}: {counts[i]}");
                            }
                            break;
                        case 7:
                            // Get Volunteer Closed Calls History
                            Console.Write("Enter volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                foreach (var closedCall in s_bl.Call.GetVolunteerClosedCallsHistory(volunteerId, null, null))
                                {
                                    Console.WriteLine(closedCall);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 8:
                            // Get Available Open Calls For Volunteer
                            Console.Write("Enter volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int openVolunteerId))
                            {
                                foreach (var openCall in s_bl.Call.GetAvailableOpenCallsForVolunteer(openVolunteerId, null, null))
                                {
                                    Console.WriteLine(openCall);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 9:
                            // Mark Assignment As Completed
                            Console.Write("Enter volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int completeVolunteerId))
                            {
                                Console.Write("Enter assignment ID: ");
                                if (int.TryParse(Console.ReadLine(), out int assignmentId))
                                {
                                    s_bl.Call.MarkAssignmentAsCompleted(completeVolunteerId, assignmentId);
                                    Console.WriteLine("Assignment marked as completed.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid assignment ID.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 10:
                            // Cancel Assignment
                            Console.Write("Enter requester ID: ");
                            if (int.TryParse(Console.ReadLine(), out int requesterId))
                            {
                                Console.Write("Enter assignment ID: ");
                                if (int.TryParse(Console.ReadLine(), out int cancelAssignmentId))
                                {
                                    s_bl.Call.CancelAssignment(requesterId, cancelAssignmentId);
                                    Console.WriteLine("Assignment canceled.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid assignment ID.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid requester ID.");
                            }
                            break;
                        case 11:
                            // Assign Call To Volunteer
                            Console.Write("Enter volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int assignVolunteerId))
                            {
                                Console.Write("Enter call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int assignCallId))
                                {
                                    s_bl.Call.AssignCallToVolunteer(assignVolunteerId, assignCallId);
                                    Console.WriteLine("Call assigned to volunteer.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid call ID.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid volunteer ID.");
                            }
                            break;
                        case 0:
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
