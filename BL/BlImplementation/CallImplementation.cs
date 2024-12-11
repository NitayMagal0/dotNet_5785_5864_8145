using DO;
using Helpers;

namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(Call call)
    {
        throw new NotImplementedException();
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void CancelAssignment(int requesterId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallCountsByStatus()
    {
        // Retrieve all calls from the data access layer
        var calls = _dal.Call.ReadAll().ToList();

        // Group calls by their status and count the number of calls in each group
        var groupedCalls = calls
            .GroupBy(call => call.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToList();

        // Initialize an array to hold the counts for each status
        int[] callCounts = new int[Enum.GetValues(typeof(CallStatus)).Length];

        // Populate the array with the counts from the grouped calls
        foreach (var group in groupedCalls)
        {
            callCounts[(int)group.Status] = group.Count;
        }

        return callCounts;
    }

    public Call GetCallDetails(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> GetFilteredAndSortedCalls(Enum? filterField, object? filterValue, Enum? sortField)
    {
        // Retrieve all calls from the data access layer
        var calls = _dal.Call.ReadAll().ToList();

        // Retrieve all assignments from the data access layer
        var assignments = _dal.Assignment.ReadAll().ToList();

        // Filter the calls if filterField and filterValue are provided
        if (filterField != null && filterValue != null)
        {
            calls = calls.Where(call =>
            {
                var property = typeof(Call).GetProperty(filterField.ToString());
                return property != null && property.GetValue(call)?.Equals(filterValue) == true;
            }).ToList();
        }

        // Sort the calls if sortField is provided
        if (sortField != null)
        {
            calls = calls.OrderBy(call =>
            {
                var property = typeof(Call).GetProperty(sortField.ToString());
                return property?.GetValue(call);
            }).ToList();
        }
        else
        {
            // Default sorting by CallId
            calls = calls.OrderBy(call => call.Id).ToList();
        }

        // Ensure each call appears only once with its last assignment (if any)
        var uniqueCalls = calls.GroupBy(call => call.Id)
            .Select(group => group.Last())
            .ToList();

        // Convert DO.Call to BO.CallInList
        var callInList = uniqueCalls.Select(call =>
        {
            var lastAssignment = assignments.LastOrDefault(a => a.CallId == call.Id);
            return new CallInList
            {
                Id = call.Id,
                CallId = call.Id,
                CallType = (CallType)call.CallType,
                OpeningTime = call.OpeningTime,
                RemainingTime = call.MaxCompletionTime.HasValue ? call.MaxCompletionTime.Value - DateTime.Now : (TimeSpan?)null,
                LastVolunteer = lastAssignment.VolunteerId,
                TotalTime = lastAssignment.ActualEndTime.HasValue == true ? lastAssignment.ActualEndTime.Value - lastAssignment.EntryTime : (TimeSpan?)null,
                Status = (CallStatus?)call.Status,
                AssignmentsCount = assignments.Count(a => a.CallId == call.Id)
            };
        }).ToList();

        return callInList;
    }

    public IEnumerable<ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public void MarkAssignmentAsCompleted(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void UpdateCall(Call call)
    {
        throw new NotImplementedException();
    }
}
