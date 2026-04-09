import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import { UserPlus } from 'lucide-react';

const Register = () => {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    role: 'User'
  });
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await api.post('/Auth/register', formData);
      navigate('/login');
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed');
    }
  };

  return (
    <div className="page-container">
      <div className="glass-card fade-in" style={{ width: '400px', padding: '40px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '24px' }}>
          <div className="btn-primary" style={{ padding: '10px' }}><UserPlus size={24} /></div>
          <h2>Register</h2>
        </div>
        
        {error && <p style={{ color: 'var(--danger)', marginBottom: '16px' }}>{error}</p>}
        
        <form onSubmit={handleSubmit}>
          <label style={{ display: 'block', marginBottom: '8px', color: 'var(--text-muted)' }}>Full Name</label>
          <input 
            type="text" 
            className="input-field" 
            value={formData.fullName} 
            onChange={(e) => setFormData({...formData, fullName: e.target.value})} 
            required 
          />

          <label style={{ display: 'block', marginBottom: '8px', color: 'var(--text-muted)' }}>Email</label>
          <input 
            type="email" 
            className="input-field" 
            value={formData.email} 
            onChange={(e) => setFormData({...formData, email: e.target.value})} 
            required 
          />
          
          <label style={{ display: 'block', marginBottom: '8px', color: 'var(--text-muted)' }}>Password</label>
          <input 
            type="password" 
            className="input-field" 
            value={formData.password} 
            onChange={(e) => setFormData({...formData, password: e.target.value})} 
            required 
          />

          <label style={{ display: 'block', marginBottom: '8px', color: 'var(--text-muted)' }}>Role</label>
          <select 
            className="input-field" 
            value={formData.role} 
            onChange={(e) => setFormData({...formData, role: e.target.value})}
          >
            <option value="User">User</option>
            <option value="Admin">Admin</option>
          </select>
          
          <button type="submit" className="btn-primary" style={{ width: '100%', marginTop: '10px' }}>
            Create Account
          </button>
        </form>
        
        <p style={{ marginTop: '20px', textAlign: 'center', color: 'var(--text-muted)' }}>
          Already have an account? <Link to="/login" style={{ color: 'var(--primary)', textDecoration: 'none' }}>Login</Link>
        </p>
      </div>
    </div>
  );
};

export default Register;
