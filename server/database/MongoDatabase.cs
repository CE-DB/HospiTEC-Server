using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HospiTec_Server.database
{
    public class MongoDatabase
    {
        public MongoClient client;
        public IMongoDatabase db;
        public IMongoCollection<StaffEvaluation> staffCollection;
        public IMongoCollection<HospitalEvaluation> hospitalCollection;
        
        public MongoDatabase()
        {

            client = new MongoClient("mongodb://172.26.15.187:27017");
            db = client.GetDatabase("hospitec");
            
            this.staffCollection = db.GetCollection<StaffEvaluation>("staffEvaluation");
            this.hospitalCollection = db.GetCollection<HospitalEvaluation>("hospitalEvaluation");

        }

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