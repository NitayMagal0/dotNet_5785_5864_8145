using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal class VolunteerManager
    {
        private static readonly DalApi.IDal _dal = DalApi.Factory.Get;

        /// <summary>
        /// Convert BO.Volunteer to DO.Volunteer
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        internal static DO.Volunteer ConvertVolunteerToDO(BO.Volunteer volunteer)
        {
            return new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                MobilePhone = volunteer.MobilePhone,
                Email = volunteer.Email,
                Password = volunteer.Password,
                FullAddress = volunteer.FullAddress,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                Role = MapRole(volunteer.Role),
                IsActive = volunteer.IsActive,
                MaxDistanceForCall = volunteer.MaxDistanceForCall,
                DistanceType = MapDistanceType(volunteer.DistanceType)
            };
        }

        /// <summary>
        /// Convert Enum Role from BO to DO
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static DO.Role MapRole(BO.Role role)
        {
            return role switch
            {
                BO.Role.Manager => DO.Role.Manager,
                BO.Role.Volunteer => DO.Role.Volunteer,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }

        /// <summary>
        /// Convert Enum DistanceType from BO to DO
        /// </summary>
        /// <param name="distanceType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static DO.DistanceType MapDistanceType(BO.DistanceType distanceType)
        {
            return distanceType switch
            {
                BO.DistanceType.AirDistance => DO.DistanceType.AirDistance,
                BO.DistanceType.WalkingDistance => DO.DistanceType.WalkingDistance,
                BO.DistanceType.DrivingDistance => DO.DistanceType.DrivingDistance,
                _ => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
            };
        }

        /// <summary>
        /// Convert DO.Volunteer to BO.Volunteer
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        internal static BO.Volunteer ConvertVolunteerToBO(DO.Volunteer volunteer)
        {
            return new BO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.FullName,
                MobilePhone = volunteer.MobilePhone,
                Email = volunteer.Email,
                Password = volunteer.Password,
                FullAddress = volunteer.FullAddress,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                Role = MapRole(volunteer.Role),
                IsActive = volunteer.IsActive,
                MaxDistanceForCall = volunteer.MaxDistanceForCall,
                DistanceType = MapDistanceType(volunteer.DistanceType)
            };
        }
        /// <summary>
        /// Convert Enum Role from DO to BO
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static BO.Role MapRole(DO.Role role)
        {
            return role switch
            {
                DO.Role.Manager => BO.Role.Manager,
                DO.Role.Volunteer => BO.Role.Volunteer,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }

        /// <summary>
        /// Convert Enum DistanceType from DO to BO
        /// </summary>
        /// <param name="distanceType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static BO.DistanceType MapDistanceType(DO.DistanceType distanceType)
        {
            return distanceType switch
            {
                DO.DistanceType.AirDistance => BO.DistanceType.AirDistance,
                DO.DistanceType.WalkingDistance => BO.DistanceType.WalkingDistance,
                DO.DistanceType.DrivingDistance => BO.DistanceType.DrivingDistance,
                _ => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
            };
        }

        /// <summary>
        /// Retrieves the details of a volunteer by their ID.
        /// </summary>
        /// <param name="id">The ID of the volunteer.</param>
        /// <returns>A BO.Volunteer object containing the volunteer's details.</returns>
        /// <exception cref="Exception">Thrown when the volunteer does not exist.</exception>
        internal BO.Volunteer GetVolunteerDetails(int id)
        {
            var volunteer = _dal.Volunteer.Read(id);
            if (volunteer == null)
                throw new Exception("Volunteer doesn't exist");
            return ConvertVolunteerToBO(volunteer);
        }
      
    }
}
