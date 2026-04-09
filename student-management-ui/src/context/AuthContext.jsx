import { createContext, useContext, useState, useEffect } from 'react';
import api from '../services/api';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      // Decode user from token if needed, or set a basic user state
      setUser({ token });
    }
    setLoading(false);
  }, []);

  const login = async (email, password) => {
    const response = await api.post('/Auth/login', { email, password });
    const { accessToken, fullName, role } = response.data.data;
    localStorage.setItem('accessToken', accessToken);
    setUser({ fullName, role, token: accessToken });
    return response.data;
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
