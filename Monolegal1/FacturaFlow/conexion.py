import os
from dotenv import load_dotenv
from pymongo.mongo_client import MongoClient
from pymongo.server_api import ServerApi

# Cargar las variables del archivo .env
load_dotenv()

# Extraer la cadena de conexión
uri = os.getenv("MONGO_URI")

# Crear el cliente de conexión
cliente = MongoClient(uri, server_api=ServerApi('1'))

try:
    # Confirmar la conexión
    cliente.admin.command('ping')
    print("¡Conexión exitosa a MongoDB Atlas!")
except Exception as e:
    print("Error de conexión:", e)