using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HospiTec_Server.database
{
    /// <summary>
    /// This class manage operations 
    /// for hospital and staff evaluation in mongodb database
    /// </summary>
    public class MongoDatabase
    {
        public MongoClient client;
        public IMongoDatabase db;
        /// <summary>
        /// Maps tha staff evaluation entity
        /// </summary>
        public IMongoCollection<StaffEvaluation> staffCollection;
        /// <summary>
        /// Maps tha hospital evaluation entity
        /// </summary>
        public IMongoCollection<HospitalEvaluation> hospitalCollection;
        
        /// <summary>
        /// This sets the connection with the server
        /// </summary>
        /// <param name="configuration">Class to get connections string from appsettings</param>
        public MongoDatabase(IConfiguration configuration)
        {

            client = new MongoClient(configuration.GetConnectionString("mongodb"));
            db = client.GetDatabase("hospitec");
            
            this.staffCollection = db.GetCollection<StaffEvaluation>("staffEvaluation");
            this.hospitalCollection = db.GetCollection<HospitalEvaluation>("hospitalEvaluation");

        }

        /// <summary>
        /// Inserts new staff evaluation.
        /// </summary>
        /// <param name="patient_id">Id of the patient who is evaluating</param>
        /// <param name="staff_id">Id of the staff to evaluate</param>
        /// <param name="evaluation">Evaluation in scale between 1 and 5</param>
        /// <param name="date">Date of evaluation</param>
        /// <returns>indicator if the insert is succesful (true) or if it was a fail (false)</returns>
        public bool generateStaffEvaluation(string patient_id, string staff_id, int evaluation, DateTime date)
        {
            var result = true;
            try
            {
                staffCollection.InsertOne(new StaffEvaluation(patient_id, staff_id, evaluation, date));
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine(e);
                result = false;
            }
            
            return result;
        }

        /// <summary>
        /// Inserts new hospital evaluation.
        /// </summary>
        /// <param name="patient_id">Id of the patient who is evaluating</param>
        /// <param name="category">Type of element in evaluation</param>
        /// <param name="evaluation">Evaluation points in scale between 1 and 5</param>
        /// <param name="date">Date of evaluation</param>
        /// <returns>Indicator if the insert is succesful (true) or if it was a fail (false)</returns>
        public bool generateHospitalEvaluation(string patient_id, int category, int evaluation, DateTime date)
        {
            var result = true;
            try
            {
                hospitalCollection.InsertOne(new HospitalEvaluation(patient_id, category, evaluation, date));
            }
            catch (MongoWriteException e)
            {
                Console.WriteLine(e);
                result = false;
            }

            return result;
        }  
    }

    /// <summary>
    /// Maps the hospital evaluation entity
    /// </summary>
    public class HospitalEvaluation
    {
        public ObjectId _id { get; set; }
        public string patient_id { get; set; }
        public int category { get; set; }
        public int evaluation { get; set; }
        public DateTime date { get; set; }

        public HospitalEvaluation(string __patient_id, int __catetory, int __evaluation, DateTime __date)
        {
            this.patient_id = __patient_id;
            this.category = __catetory;
            this.evaluation = __evaluation;
            this.date = __date;
        }
    }
    /// <summary>
    /// Maps the staff evaluation entity
    /// </summary>
    public class StaffEvaluation
    {
        public ObjectId _id { get; set; }
        public string patient_id { get; set; }
        public string staff_id { get; set; }
        public int evaluation { get; set; }
        public DateTime date { get; set; }

        public StaffEvaluation(string __patient_id, string __staff_id, int __evaluation, DateTime __date)
        {
            this.patient_id = __patient_id;
            this.evaluation = __evaluation;
            this.staff_id = __staff_id;
            this.date = __date;
        }
    }
    
}