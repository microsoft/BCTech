from flask import Flask, request, jsonify, abort, Response, render_template
import os
import xml.etree.ElementTree as ET    
  
app = Flask(__name__)  
  
# Configuration  
UPLOAD_FOLDER = 'api/uploads'  
if not os.path.exists(UPLOAD_FOLDER):  
    os.makedirs(UPLOAD_FOLDER)  
  
BEARER_TOKEN = 'secret'  

@app.route('/')
def index():
   return render_template('index.html')

# obsolete
@app.route('/demo-api/', methods=['POST'])  
def receive_xml():  
    # Check for Bearer token  
    auth_header = request.headers.get('Authorization')  
    if auth_header is None or not auth_header.startswith('Bearer '):  
        abort(401, description="Unauthorized: No Bearer token found")  
      
    token = auth_header.split(" ")[1]  
    if token != BEARER_TOKEN:  
        abort(401, description="Unauthorized: Invalid Bearer token")  
  
    # # Check if the content type is XML  
    if request.content_type != 'application/xml':  
        abort(400, description="Bad Request: Content type must be 'application/xml'")  
  
    xml_data = request.data  
  
    # Save the XML data to a file  
    file_path = os.path.join(UPLOAD_FOLDER, 'data.xml')  
    with open(file_path, 'wb') as f:  
        f.write(xml_data)  
    
    print("Stored: " + file_path)
    return jsonify({"message": "XML data received and stored successfully"}), 200  


@app.route('/demo-api/getresponse', methods=['GET'])  
def get_response():  
    # Check for Bearer token  
    auth_header = request.headers.get('Authorization')  
    if auth_header is None or not auth_header.startswith('Bearer '):  
        abort(401, description="Unauthorized: No Bearer token found")  
      
    token = auth_header.split(" ")[1]  
    if token != BEARER_TOKEN:  
        abort(401, description="Unauthorized: Invalid Bearer token") 

    service_header = request.headers.get('Service')  
    if service_header is None:
        abort(401, description="Missing: Service header")  

    # Get the invoice ID from the query parameters  
    invoice_id = request.args.get('invoice_id')  
      
    if not invoice_id:  
        return jsonify({"message": "invoice_id parameter is required"}), 400  
  
    # Check if a file with the name of the invoice ID exists  
    file_path = f"api/uploads/{service_header}/{invoice_id}.xml" 
    
    if os.path.isfile(file_path):  
        print("Got: " + file_path)
        return jsonify({"message": "Success", "flag" : "Prater"}), 200  
    else:  
        return jsonify({"message": "Not Received"}), 200  
    

@app.route('/demo-api/send', methods=['POST'])  
def send():  
    # Check for Bearer token  
    auth_header = request.headers.get('Authorization')  
    if auth_header is None or not auth_header.startswith('Bearer '):  
        abort(401, description="Unauthorized: No Bearer token found")  
      
    token = auth_header.split(" ")[1]  
    if token != BEARER_TOKEN:  
        abort(401, description="Unauthorized: Invalid Bearer token")  
  
    service_header = request.headers.get('Service')  
    if service_header is None:
        abort(401, description="Missing: Service header")  

    # # Check if the content type is XML  
    if request.content_type != 'application/xml':  
        abort(400, description="Bad Request: Content type must be 'application/xml'")  
  
    xml_data = request.data  

    # Parse the XML data  
    root = ET.fromstring(xml_data)  
  
    # Define the namespace  
    namespace = {'cbc': 'urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2'}  

    # Find the ID element  
    id_element = root.find('cbc:ID', namespace)  

    # Extract the ID value  
    if id_element is not None:  
        invoice_id = id_element.text  
        print(f"Recevied Invoice ID: {invoice_id}")  
    
        # Save the XML data to a file  
        directory = os.path.join(UPLOAD_FOLDER, service_header)
        os.makedirs(directory, exist_ok=True)
        file_path = os.path.join(UPLOAD_FOLDER + '/' + service_header,  f'{invoice_id}.xml')  
        with open(file_path, 'wb') as f:  
            f.write(xml_data)  
        
        print("Stored: " + file_path)
    
    
    return jsonify({"message": f"Received invoice {invoice_id}", "flag" : "Hofburg"}), 200  

  
@app.route('/demo-api/receive', methods=['GET'])  
def receive_file():  
   
    service_header = request.headers.get('Service')  
    if service_header is None:
        abort(401, description="Missing: Service header") 

    fodler = UPLOAD_FOLDER + '/' + service_header
    # Get the list of files in the directory  
    files = [os.path.join(fodler, f) for f in os.listdir(fodler) if os.path.isfile(os.path.join(fodler, f))]  
      
    if not files:  
        return jsonify({"message": "No files found in upload directory"}), 404  
  
    # Find the most recent file  
    most_recent_file = max(files, key=os.path.getctime)  
      
    try:  
        # Read the content of the most recent file  
        with open(most_recent_file, 'rb') as file:  
            file_content = file.read()  

        print("Recived: File")

        # Return the file content in the response  
        return Response(file_content, mimetype='application/xml')  
  
    except Exception as e:  
        return jsonify({"message": "Error reading file", "error": str(e)}), 500 

@app.route('/demo-api/approve', methods=['GET'])  
def approve():  

    # Check for Bearer token  
    auth_header = request.headers.get('Authorization')  
    if auth_header is None or not auth_header.startswith('Bearer '):  
        abort(401, description="Unauthorized: No Bearer token found")  

    service_header = request.headers.get('Service')  
    if service_header is None:
        abort(401, description="Missing: Service header") 

    return jsonify({"message": "Received invoice", "flag": "Sacher"}), 200  


@app.route('/demo-api/customaction', methods=['GET'])  
def customaction():  

    # Check for Bearer token  
    auth_header = request.headers.get('Authorization')  
    if auth_header is None or not auth_header.startswith('Bearer '):  
        abort(401, description="Unauthorized: No Bearer token found")  
   
    service_header = request.headers.get('Service')  
    if service_header is None:
        abort(401, description="Missing: Service header") 

    return jsonify({"message": "Received invoice"}), 200  


@app.route('/Hofburg/Prater/Sacher/', methods=['GET']) 
def win():
    return render_template('win.html')

  
# if __name__ == '__main__':  
#     app.run(debug=True, host='localhost', port=5000)  

if __name__ == '__main__':
   app.run()