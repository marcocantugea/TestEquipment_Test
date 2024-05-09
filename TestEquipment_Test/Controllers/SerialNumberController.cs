using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using TestEquipment_Test.Models.Data;
using SqlCommand = System.Data.SqlClient.SqlCommand;
using SqlConnection = System.Data.SqlClient.SqlConnection;
using SqlDataAdapter = System.Data.SqlClient.SqlDataAdapter;
using SqlDataReader = System.Data.SqlClient.SqlDataReader;

namespace TestEquipment_Test.Controllers
{
    [ApiController]
    [Route("serialnumber")]
    public class SerialNumberController : ControllerBase
    {
        private readonly string _connectionString;
        SqlCommand? command;
        SqlDataReader? reader;
        SqlConnection? connection;
        string message = "";
        string select = "";
        bool serial_number = false;
        bool SW_DMR_Process_Only = true;
        bool SW_DMR_Procees_Only_Exception = true;
        string DMRProcessID = "";
        string OrderNumber = "";
        string ProductQuantity = "";
        bool isSerialScrapped = false;
        int ProductTrackID = 0;
        string StationWorkCenter = "";
        string LineID = "";
        string PlantID = "";
        bool CheckOperationsStatus = true;
        int ProductStatus = 0;
        string PreviousStationName = "";
        string WorkCenter = "";
        int SelectedSequenceNumber = -1;
        int LastProductStatus;
        string LastStation = "";
        string LastStationOrder = "";
        string LastLineID = "";
        string LastAreaID = "";
        string LastWorkCenter = "";
        string PreviousProductStatus = "";
        string _Station = "";
        string _StationOrder = "";
        string _LineID = "";
        string _AreaID = "";
        string _WorkCenter = "";

        public SerialNumberController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public IActionResult ValidateSerialNumber(string serialnumber, string stationid, string EmployeeId)
        {

            if (serialnumber != "" || stationid != "" || EmployeeId != "")
            {
                string validateSerial = ValidateSerialNumbers(serialnumber);
                if (validateSerial == "SERIAL_EXISTS")
                {
                    GetIDs(stationid);
                    //CHECK OTHERS VALIDATIONS
                    string checkpreviousccw = CheckPreviousCCWWs(serialnumber, stationid, LineID, PlantID);                    
                    if (checkpreviousccw == "OrderID")
                    {
                        serial_number = true;
                        message = checkpreviousccw;

                        int Checklist = 0;
                        Checklist = Checklist_By_SerialNumber_Lock(serialnumber, stationid, EmployeeId);
                        if (Checklist == 0)
                        {
                            serial_number = false;
                            message = "CHECKLIST_NOT_FILLEDOUT";
                        }

                    }
                    else if (checkpreviousccw == "ONLY_PRODUCTS_WITHROUTE_ACTIVE")
                    {
                        serial_number = false;
                        message = checkpreviousccw;
                    }
                    else if (checkpreviousccw == "SERIALNUMBER_SCRAPPED")
                    {
                        serial_number = false;
                        message = checkpreviousccw;
                    }
                    else
                    {
                        serial_number = false;
                        message = checkpreviousccw;
                    }


                }
                else if (validateSerial == "SERIAL_NOEXISTS")
                {
                    serial_number = false;
                    message = validateSerial;
                }
                else
                {
                    serial_number = false;
                    message = validateSerial;
                }

                //Creacion del json devuelto
                var response = new SerialNumber_
                {
                    success = serial_number.ToString(),
                    message = message
                };

                return Ok(response);
            }
            else
            {
                return BadRequest("NO_DATA_INTRODUCED");
            }

        }

        private string ValidateSerialNumbers(string serialnumber)
        {
            string res = "";
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (command = new SqlCommand("SP_FWDAPI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Option", "CheckSerialNumber");
                    command.Parameters.AddWithValue("@Value", serialnumber);


                    using (reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            select = reader.GetString(0); // Mensaje recibido del select

                            if (select == "EXISTS") // Si el select es exists entonces existe ese sn, si no, es un dont exists
                            {
                                connection.Close();
                                res = "SERIAL_EXISTS";

                            }
                            else if (select == "DONT_EXISTS")
                            {
                                res = "SERIAL_NOEXISTS";
                            }

                        }
                        else
                        {
                            res = "ERROR";
                        }
                    }
                }
            }
            return res;
        }

        private void GetIDs(string stationid)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (command = new SqlCommand("SP_FWDAPI", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Option", "GetIDs");
                    command.Parameters.AddWithValue("@Value", stationid);

                    using (reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LineID = reader.GetString(0);
                            PlantID = reader.GetString(1);
                            WorkCenter = reader.GetString(2);
                            connection.Close();
                        }
                    }
                }
            }
        }

        private string CheckPreviousCCWWs(string serialnumber, string stationid, string LineID, string PlantID)
        {
            string res = "";

            if (CheckOperationsStatus)
            {
                    using (connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        using (command = new SqlCommand("SP_ProductionTracking", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            if (ProductStatus.Equals("1") || ProductStatus.Equals("4") || ProductStatus.Equals("2"))
                            {
                                command.Parameters.AddWithValue("SP_Operation", "Check_Station_Order");
                            }
                            else
                            {
                                command.Parameters.AddWithValue("SP_Operation", "Is_Product_Started");
                            }
                            command.Parameters.AddWithValue("SequenceNumber", SelectedSequenceNumber);
                            command.Parameters.AddWithValue("SW_Glendale", "False");
                            command.Parameters.AddWithValue("SerialNumber", serialnumber);
                            command.Parameters.AddWithValue("StationOrder", _StationOrder);
                            command.Parameters.AddWithValue("LineID", LineID);
                            command.Parameters.AddWithValue("Workcenter", WorkCenter);
                            command.Parameters.AddWithValue("DMRProcessID", DMRProcessID);
                            command.Parameters.AddWithValue("SW_Check_Previous_Station_Operation", "True");
                            DataSet dsData = new DataSet();
                            SqlDataAdapter adapter = new SqlDataAdapter(command);

                            adapter.Fill(dsData);
                            connection.Close();

                            if (dsData.Tables.Count > 0)
                            {
                                if (!CheckOperationsStatus)
                                {
                                    if (dsData.Tables[0].Rows.Count == 0)
                                    {
                                        //La operacion anterior a esta no puedes continuar sin acabar la anterior
                                        res = "OPERATION_BEFOREPENDING";
                                    }
                                }
                                else
                                {
                                    if (dsData.Tables[0].Rows.Count == 0)
                                    {
                                        if (dsData.Tables[1].Rows.Count == 0)
                                        {
                                            //La operacion anterior a esta no puedes continuar sin acabar la anterior
                                            res = "OPERATION_BEFOREPENDING";
                                        }
                                        else
                                        {
                                            PreviousStationName = dsData.Tables[1].Rows[0][0].ToString();
                                            res = "OPERATION_BEFOREPENDING: " + PreviousStationName;
                                            //Termina la estacion anterior
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (DMR_IsSerialInDMR(serialnumber, stationid))
                    {
                        string StatusSerialNumber = GetSerialNumber(serialnumber, stationid, LineID, PlantID, DMRProcessID);
                        if (StatusSerialNumber != "ERROR")
                        {
                            res = StatusSerialNumber;
                        }
                        if (isSerialScrapped == true)
                        {
                            res = "SERIALNUMBER_SCRAPPED";
                        }
                    }
                    else
                    {
                        if (SW_DMR_Process_Only)
                        {
                            if (SW_DMR_Procees_Only_Exception)
                            {
                                if (SerialIsDMRProcessException(serialnumber, stationid) == false)
                                {
                                    res = "ONLY_PRODUCTS_WITHROUTE_ACTIVE";
                                }
                            }
                            else
                            {
                                res = "ONLY_PRODUCTS_WITHROUTE_ACTIVE";
                            }
                        }
                    }

                }

            return res;
        }

        private bool DMR_IsSerialInDMR(string serNum, string StationID)
        {
            bool bRet = false;
            int ProductTrackID = 0;
            SelectedSequenceNumber = -1;

            bool cmbSequence = true;

            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SP_ProductionTracking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SP_Operation", "DMR_IsOpen");
                        command.Parameters.AddWithValue("@stationid", StationID);
                        command.Parameters.AddWithValue("@SerialNumber", serNum);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        DataSet DMR_StationWorkCenters;
                        DMR_StationWorkCenters = dataSet;
                        connection.Close();

                        bool MsgDMRStationDoesnotHaveRelated_WS = false;

                        if (DMR_StationWorkCenters != null && DMR_StationWorkCenters.Tables.Count > 0 && DMR_StationWorkCenters.Tables[0].Rows.Count > 0 && DMR_StationWorkCenters.Tables[3].Rows.Count > 0)
                        {
                            DMRProcessID = DMR_StationWorkCenters.Tables[0].Rows[0]["DMRProcessID"].ToString();
                            OrderNumber = DMR_StationWorkCenters.Tables[3].Rows[0]["OrderID"].ToString();
                            ProductQuantity = DMR_StationWorkCenters.Tables[3].Rows[0]["Qty"].ToString();
                        }

                        if (!string.IsNullOrEmpty(DMRProcessID.Trim()))
                        {
                            cmbSequence = false;

                            if (DMR_StationWorkCenters.Tables[1].Rows.Count == 0)
                            {
                                MsgDMRStationDoesnotHaveRelated_WS = true;
                            }

                            if (DMR_StationWorkCenters.Tables.Count > 1 && DMR_StationWorkCenters.Tables[2].Rows.Count > 0)
                            {
                                ProductStatus = (int)DMR_StationWorkCenters.Tables[2].Rows[0]["StatusID"];
                                ProductTrackID = (int)DMR_StationWorkCenters.Tables[2].Rows[0]["ProductTrackID"];

                                if (ProductStatus == 7)
                                    ProductStatus = 3;
                            }

                            if (DMR_StationWorkCenters.Tables.Count > 0 && DMR_StationWorkCenters.Tables[1].Rows.Count > 0)
                            {
                                SelectedSequenceNumber = (int)DMR_StationWorkCenters.Tables[1].Rows[0]["OperationNumber"];
                                cmbSequence = true;

                                using (SqlCommand verifyCommand = new SqlCommand("SP_ProductionTracking", connection))
                                {
                                    verifyCommand.CommandType = CommandType.StoredProcedure;

                                    verifyCommand.Parameters.AddWithValue("@SP_Operation", "DMR_VerifySequence");
                                    verifyCommand.Parameters.AddWithValue("@SerialNumber", serNum);
                                    verifyCommand.Parameters.AddWithValue("@DMRProcessID", DMRProcessID);
                                    verifyCommand.Parameters.AddWithValue("@SequenceNumber", SelectedSequenceNumber);
                                    verifyCommand.Parameters.AddWithValue("@stationid", StationID);

                                    DataSet dsVerify = new DataSet();
                                    connection.Open();
                                    adapter.SelectCommand = verifyCommand;
                                    adapter.Fill(dsVerify);
                                    connection.Close();

                                    if (dsVerify != null && dsVerify.Tables.Count > 0 && dsVerify.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(dsVerify.Tables[0].Rows[0]["SequenceNumber"].ToString()))
                                    {
                                        DataRow[] drArr = DMR_StationWorkCenters.Tables[1].Select($"OperationNumber={dsVerify.Tables[0].Rows[0]["SequenceNumber"]} and OperationOrder={dsVerify.Tables[0].Rows[0]["DMROrder"]}");
                                        if (drArr != null && drArr.Length > 0)
                                        {
                                            SelectedSequenceNumber = (int)dsVerify.Tables[0].Rows[0]["SequenceNumber"];
                                            cmbSequence = false;
                                        }
                                    }
                                }
                            }

                            bRet = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bRet = false;
            }

            return bRet;
        }

        private bool SerialIsDMRProcessException(string serNum, string StationID)
        {
            bool bRet = false;

            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SP_ProductionTracking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SP_Operation", "CheckOnlyDMRException");
                        command.Parameters.AddWithValue("@StationID", StationID);
                        command.Parameters.AddWithValue("@SerialNumber", serNum);

                        using (reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                connection.Close();
                                bRet = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bRet = false;
            }

            return bRet;
        }

        private string GetSerialNumber(string serialnumber, string StationID, string LineID, string PlantID, string DMRProcessID)
        {
            string res = "";
            try
            {
                int counter = 0;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SP_ProductionTracking", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SP_Operation", "SERIAL_NUMBER_INFORMATION");
                    command.Parameters.AddWithValue("@SerialNumber", serialnumber);
                    command.Parameters.AddWithValue("@StationID", StationID);
                    command.Parameters.AddWithValue("@LineID", LineID);
                    command.Parameters.AddWithValue("@PlantID", PlantID);
                    command.Parameters.AddWithValue("@SW_Glendale", "False");
                    command.Parameters.AddWithValue("@DMRProcessID", DMRProcessID);
                    command.Parameters.AddWithValue("@SW_Virtual_Labels_Tracking", "False");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dsSerialNumberInfo = new DataSet();
                        adapter.Fill(dsSerialNumberInfo);
                        connection.Close();
                        try
                        {
                            counter = dsSerialNumberInfo.Tables[0].Rows.Count;
                            OrderNumber = dsSerialNumberInfo.Tables[0].Rows[0]["OrderID"].ToString();
                        }
                        catch (Exception ex)
                        {
                            counter = 0;
                        }

                        if (counter > 0)
                        {
                            int ReworkCountsByStation = 0;
                            for (int count = 0; count < dsSerialNumberInfo.Tables[1].Rows.Count; count++)
                            {
                                if (dsSerialNumberInfo.Tables[1].Rows[count]["Status"] == "6")
                                {
                                    ReworkCountsByStation++;
                                }
                            }

                            // Check for DMR
                            if (dsSerialNumberInfo.Tables[5].Rows.Count > 0 || dsSerialNumberInfo.Tables[2].Rows.Count > 0 || dsSerialNumberInfo.Tables[2].Rows.Count > 0)
                            {
                                isSerialScrapped = true;
                                ProductTrackID = Convert.ToInt32(dsSerialNumberInfo.Tables[2].Rows[0]["ProductTrackID"]);
                            }

                            if (dsSerialNumberInfo.Tables[4].Rows.Count > 0)
                            {
                                StationWorkCenter = dsSerialNumberInfo.Tables[4].Rows[0]["Work_Center"].ToString();
                            }

                            res = "OrderID: " + OrderNumber + "ProductTrackID: " + ProductTrackID + "- WorkStation:" + StationWorkCenter + " - ReworkByStation: " + ReworkCountsByStation;

                        }
                        else
                        {
                            res = "NOT_VALID_SERIALNUMBER";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                res = "ERROR";
            }
            return res;
        }

        private int Checklist_By_SerialNumber_Lock(string serialnumber, string StationID, string EmployeeId)
        {
            int counter = 0;
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (command = new SqlCommand("SP_ProductionTracking", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SP_Operation", "VerifySerialNumberCheckList");
                    command.Parameters.AddWithValue("@SerialNumber", serialnumber);
                    command.Parameters.AddWithValue("@StationID", StationID);
                    command.Parameters.AddWithValue("@EmployeeID", EmployeeId);

                    try
                    {

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataSet dsSerialNumberInfo = new DataSet();
                            adapter.Fill(dsSerialNumberInfo);
                            counter = dsSerialNumberInfo.Tables[0].Rows.Count;
                            connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        counter = 0;
                    }
                }
            }
            return counter;
        }

        



    }
}
