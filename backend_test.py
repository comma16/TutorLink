import requests
import sys
import json
from datetime import datetime

class TutorLinkAPITester:
    def __init__(self, base_url="https://login-portal-32.preview.emergentagent.com/api"):
        self.base_url = base_url
        self.token = None
        self.user_id = None
        self.tests_run = 0
        self.tests_passed = 0
        self.created_tutoria_id = None
        self.second_user_token = None
        self.second_user_id = None

    def run_test(self, name, method, endpoint, expected_status, data=None, headers=None):
        """Run a single API test"""
        url = f"{self.base_url}/{endpoint}"
        test_headers = {'Content-Type': 'application/json'}
        
        if self.token and not headers:
            test_headers['Authorization'] = f'Bearer {self.token}'
        elif headers:
            test_headers.update(headers)

        self.tests_run += 1
        print(f"\n🔍 Testing {name}...")
        print(f"   URL: {url}")
        
        try:
            if method == 'GET':
                response = requests.get(url, headers=test_headers)
            elif method == 'POST':
                response = requests.post(url, json=data, headers=test_headers)
            elif method == 'PUT':
                response = requests.put(url, json=data, headers=test_headers)
            elif method == 'DELETE':
                response = requests.delete(url, headers=test_headers)

            success = response.status_code == expected_status
            if success:
                self.tests_passed += 1
                print(f"✅ Passed - Status: {response.status_code}")
                try:
                    return success, response.json()
                except:
                    return success, {}
            else:
                print(f"❌ Failed - Expected {expected_status}, got {response.status_code}")
                try:
                    print(f"   Response: {response.json()}")
                except:
                    print(f"   Response: {response.text}")
                return False, {}

        except Exception as e:
            print(f"❌ Failed - Error: {str(e)}")
            return False, {}

    def test_user_registration(self):
        """Test user registration"""
        user_data = {
            "email": f"juan.test.{datetime.now().strftime('%H%M%S')}@test.com",
            "password": "TestPass123!",
            "first_name": "Juan",
            "last_name": "Pérez",
            "career": "Ingeniería",
            "skills": "Matemáticas,Física",
            "needs_help_in": "Historia"
        }
        
        success, response = self.run_test(
            "User Registration",
            "POST",
            "auth/register",
            200,
            data=user_data
        )
        
        if success and 'access_token' in response:
            self.token = response['access_token']
            self.user_id = response['user']['id']
            print(f"   User ID: {self.user_id}")
            return True
        return False

    def test_user_login(self):
        """Test user login with existing credentials"""
        # First register a user
        user_data = {
            "email": f"maria.test.{datetime.now().strftime('%H%M%S')}@test.com",
            "password": "TestPass123!",
            "first_name": "Maria",
            "last_name": "García",
            "career": "Medicina",
            "skills": "Biología,Química",
            "needs_help_in": "Estadística"
        }
        
        # Register first
        success, response = self.run_test(
            "Register Second User",
            "POST",
            "auth/register",
            200,
            data=user_data
        )
        
        if not success:
            return False
            
        # Now test login
        login_data = {
            "email": user_data["email"],
            "password": user_data["password"]
        }
        
        success, response = self.run_test(
            "User Login",
            "POST",
            "auth/login",
            200,
            data=login_data
        )
        
        if success and 'access_token' in response:
            self.second_user_token = response['access_token']
            self.second_user_id = response['user']['id']
            return True
        return False

    def test_get_current_user(self):
        """Test getting current user info"""
        success, response = self.run_test(
            "Get Current User",
            "GET",
            "auth/me",
            200
        )
        return success

    def test_create_tutoria(self):
        """Test creating a tutoria"""
        tutoria_data = {
            "titulo": "Clases de Matemáticas Avanzadas",
            "materia": "Matemáticas",
            "descripcion": "Ofrezco clases de matemáticas avanzadas para estudiantes universitarios. Incluye cálculo diferencial e integral, álgebra lineal y estadística.",
            "precio_hora": 20.0
        }
        
        success, response = self.run_test(
            "Create Tutoria",
            "POST",
            "tutorias",
            200,
            data=tutoria_data
        )
        
        if success and 'id' in response:
            self.created_tutoria_id = response['id']
            print(f"   Created Tutoria ID: {self.created_tutoria_id}")
            return True
        return False

    def test_get_all_tutorias(self):
        """Test getting all tutorias"""
        success, response = self.run_test(
            "Get All Tutorias",
            "GET",
            "tutorias",
            200
        )
        
        if success and isinstance(response, list):
            print(f"   Found {len(response)} tutorias")
            return True
        return False

    def test_get_tutoria_details(self):
        """Test getting specific tutoria details"""
        if not self.created_tutoria_id:
            print("❌ No tutoria ID available for testing")
            return False
            
        success, response = self.run_test(
            "Get Tutoria Details",
            "GET",
            f"tutorias/{self.created_tutoria_id}",
            200
        )
        return success

    def test_search_tutorias(self):
        """Test searching tutorias"""
        success, response = self.run_test(
            "Search Tutorias",
            "GET",
            "search/tutorias?q=Matemáticas",
            200
        )
        
        if success and isinstance(response, list):
            print(f"   Found {len(response)} matching tutorias")
            return True
        return False

    def test_create_calificacion(self):
        """Test creating a rating"""
        if not self.created_tutoria_id or not self.second_user_token:
            print("❌ Missing tutoria ID or second user token for rating test")
            return False
            
        # Switch to second user to rate the tutoria
        original_token = self.token
        self.token = self.second_user_token
        
        calificacion_data = {
            "tutoria_id": self.created_tutoria_id,
            "puntaje": 5,
            "comentario": "Excelente tutor, muy claro en sus explicaciones"
        }
        
        success, response = self.run_test(
            "Create Rating",
            "POST",
            "calificaciones",
            200,
            data=calificacion_data
        )
        
        # Switch back to original user
        self.token = original_token
        return success

    def test_get_calificaciones(self):
        """Test getting ratings for a tutoria"""
        if not self.created_tutoria_id:
            print("❌ No tutoria ID available for testing")
            return False
            
        success, response = self.run_test(
            "Get Tutoria Ratings",
            "GET",
            f"tutorias/{self.created_tutoria_id}/calificaciones",
            200
        )
        
        if success and isinstance(response, list):
            print(f"   Found {len(response)} ratings")
            return True
        return False

    def test_send_message(self):
        """Test sending a message"""
        if not self.second_user_id:
            print("❌ No second user ID available for messaging test")
            return False
            
        message_data = {
            "destinatario_id": self.second_user_id,
            "texto": "Hola, me interesa tu tutoría de matemáticas. ¿Cuándo tienes disponibilidad?"
        }
        
        success, response = self.run_test(
            "Send Message",
            "POST",
            "mensajes",
            200,
            data=message_data
        )
        return success

    def test_get_inbox(self):
        """Test getting inbox messages"""
        success, response = self.run_test(
            "Get Inbox",
            "GET",
            "mensajes/inbox",
            200
        )
        
        if success and isinstance(response, list):
            print(f"   Found {len(response)} inbox messages")
            return True
        return False

    def test_get_sent_messages(self):
        """Test getting sent messages"""
        success, response = self.run_test(
            "Get Sent Messages",
            "GET",
            "mensajes/sent",
            200
        )
        
        if success and isinstance(response, list):
            print(f"   Found {len(response)} sent messages")
            return True
        return False

    def test_update_profile(self):
        """Test updating user profile"""
        profile_data = {
            "email": "juan.test.updated@test.com",
            "first_name": "Juan Carlos",
            "last_name": "Pérez González",
            "career": "Ingeniería de Sistemas",
            "skills": "Matemáticas,Física,Programación",
            "needs_help_in": "Historia,Literatura"
        }
        
        success, response = self.run_test(
            "Update Profile",
            "PUT",
            "users/profile",
            200,
            data=profile_data
        )
        return success

    def test_get_users(self):
        """Test getting all users"""
        success, response = self.run_test(
            "Get All Users",
            "GET",
            "users",
            200
        )
        
        if success and isinstance(response, list):
            print(f"   Found {len(response)} users")
            return True
        return False

    def test_delete_tutoria(self):
        """Test deleting a tutoria (only owner can delete)"""
        if not self.created_tutoria_id:
            print("❌ No tutoria ID available for deletion test")
            return False
            
        success, response = self.run_test(
            "Delete Tutoria",
            "DELETE",
            f"tutorias/{self.created_tutoria_id}",
            200
        )
        return success

def main():
    print("🚀 Starting TutorLink API Tests...")
    print("=" * 50)
    
    tester = TutorLinkAPITester()
    
    # Test sequence
    tests = [
        ("User Registration", tester.test_user_registration),
        ("User Login", tester.test_user_login),
        ("Get Current User", tester.test_get_current_user),
        ("Create Tutoria", tester.test_create_tutoria),
        ("Get All Tutorias", tester.test_get_all_tutorias),
        ("Get Tutoria Details", tester.test_get_tutoria_details),
        ("Search Tutorias", tester.test_search_tutorias),
        ("Create Rating", tester.test_create_calificacion),
        ("Get Tutoria Ratings", tester.test_get_calificaciones),
        ("Send Message", tester.test_send_message),
        ("Get Inbox", tester.test_get_inbox),
        ("Get Sent Messages", tester.test_get_sent_messages),
        ("Update Profile", tester.test_update_profile),
        ("Get All Users", tester.test_get_users),
        ("Delete Tutoria", tester.test_delete_tutoria),
    ]
    
    # Run all tests
    for test_name, test_func in tests:
        try:
            test_func()
        except Exception as e:
            print(f"❌ {test_name} failed with exception: {str(e)}")
    
    # Print final results
    print("\n" + "=" * 50)
    print(f"📊 FINAL RESULTS:")
    print(f"   Tests Run: {tester.tests_run}")
    print(f"   Tests Passed: {tester.tests_passed}")
    print(f"   Tests Failed: {tester.tests_run - tester.tests_passed}")
    print(f"   Success Rate: {(tester.tests_passed/tester.tests_run)*100:.1f}%")
    
    if tester.tests_passed == tester.tests_run:
        print("🎉 All tests passed!")
        return 0
    else:
        print("⚠️  Some tests failed. Check the output above for details.")
        return 1

if __name__ == "__main__":
    sys.exit(main())