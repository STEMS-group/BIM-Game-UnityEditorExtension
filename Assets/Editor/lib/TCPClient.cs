using System;
using System.Net.Sockets;

using UnityEngine;


public class RevitTCPClient
{
    private static RevitTCPClient instance = null;
    TcpClient client;

    private bool isActive = false;
    private string server = "127.0.0.1";

    private NetworkStream stream;


    private RevitTCPClient() { }

    public static RevitTCPClient Client
    {
        get
        {
            if (instance == null)
            {
                instance = new RevitTCPClient();
            }
            return instance;
        }
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
        }
    }

    public void ConnectToServer()
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            Int32 port = 13000;
            client = new TcpClient(server, port);

            // Get a client stream for reading and writing.
            stream = client.GetStream();

            isActive = true;

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            // Close everything.
            stream.Close();
            client.Close();
        }
    }

    public void DisconnectToServer()
    {
        // Close everything.
        stream.Close();
        client.Close();
        isActive = false;
    }

    public string sendRequest(string request)
    {
        String responseData = "Error: Message Not Received";
        try
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] req = System.Text.Encoding.ASCII.GetBytes(request);

            // Send the message to the connected TcpServer. 
            stream.Write(req, 0, req.Length);

            //Debug.Log("Sent: "+ request);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            Byte[] data = new Byte[1024];

            // String to store the response ASCII representation.
            responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data);
            responseData = responseData.Substring(0, responseData.IndexOf("$"));
            //Debug.Log("Received: " + responseData);


        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            // Close everything.
            stream.Close();
            client.Close();
        }  

        return responseData;
    }
}

