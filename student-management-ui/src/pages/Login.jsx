import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import { LogIn } from 'lucide-react';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(email, password);
      navigate('/');
    } catch (err) {
      setError(err.response?.data?.message || 'Invalid email or password');
    }
  };

  return (
    <div className="page-container">
      <div className="glass-card fade-in" style={{ width: '400px', padding: '40px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '24px' }}>
          <div className="btn-primary" style={{ padding: '10px' }}><LogIn size={24} /></div>
          <h2>Login</h2>
        </div>
        
        {error && <p style={{ color: 'var(--danger)', marginBottom: '16px' }}>{error}</p>}
        
        <form onSubmit={handleSubmit}>
          <label style={{ display: 'block', marginBottom: '8px', color: 'var(--text-muted)' }}>Email</label>
          <input 
            type="email" 
            className="input-field" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            required 
          />
          
          <label style={{ display: 'block', marginBottom: '8px', color: 'var(--text-muted)' }}>Password</label>
          <input 
            type="password" 
            className="input-field" 
            value={password} 
            onChange={(e) => setPassword(e.target.value)} 
            required 
          />
          
          <button type="submit" className="btn-primary" style={{ width: '100%', marginTop: '10px' }}>
            Sign In
          </button>
        </form>
        
        <p style={{ marginTop: '20px', textAlign: 'center', color: 'var(--text-muted)' }}>
          Don't have an account? <Link to="/register" style={{ color: 'var(--primary)', textDecoration: 'none' }}>Register</Link>
        </p>
      </div>
    </div>
  );
};

export default Login;
